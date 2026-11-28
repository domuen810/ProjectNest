import { useEffect, useRef, useState, type FormEvent } from 'react'

const API_BASE_URL = '/api'

type InitialStoreStatus = 0 | 1

type Store = {
  id: string
  brandId: string
  mallId: string
  status: number
}

type CreateStoreRequest = {
  brandId: string
  mallId: string
  status: InitialStoreStatus
}

const statusLabels: Record<number, string> = {
  0: 'Pre-opening',
  1: 'Open',
  2: 'Closed',
}

const validationFieldLabels: Record<string, string> = {
  brandId: 'Brand ID',
  BrandId: 'Brand ID',
  mallId: 'Mall ID',
  MallId: 'Mall ID',
  status: 'Status',
  Status: 'Status',
  request: 'Request',
}

function isStore(value: unknown): value is Store {
  return (
    typeof value === 'object' &&
    value !== null &&
    'id' in value &&
    typeof value.id === 'string' &&
    'brandId' in value &&
    typeof value.brandId === 'string' &&
    'mallId' in value &&
    typeof value.mallId === 'string' &&
    'status' in value &&
    typeof value.status === 'number'
  )
}

function parseStores(value: unknown): Store[] {
  if (!Array.isArray(value)) {
    throw new Error('The stores API returned an invalid response.')
  }

  const stores: Store[] = []
  for (const item of value) {
    if (!isStore(item)) {
      throw new Error('The stores API returned an invalid store.')
    }

    stores.push(item)
  }

  return stores
}

async function fetchStores(signal?: AbortSignal): Promise<Store[]> {
  const response = await fetch(`${API_BASE_URL}/stores`, { signal })
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}.`)
  }

  return parseStores(await response.json())
}

function getErrorMessage(error: unknown): string {
  return error instanceof Error
    ? error.message
    : 'An unexpected error occurred.'
}

async function getApiErrorMessages(response: Response): Promise<string[]> {
  let responseBody: unknown
  try {
    responseBody = await response.json()
  } catch {
    return [`Request failed with status ${response.status}.`]
  }

  if (typeof responseBody !== 'object' || responseBody === null) {
    return [`Request failed with status ${response.status}.`]
  }

  if ('errors' in responseBody && typeof responseBody.errors === 'object' && responseBody.errors !== null) {
    const messages = Object.entries(responseBody.errors).flatMap(
      ([field, fieldErrors]) => {
        if (!Array.isArray(fieldErrors)) {
          return []
        }

        const label = validationFieldLabels[field] ?? field
        return fieldErrors
          .filter((message): message is string => typeof message === 'string')
          .map((message) => `${label}: ${message}`)
      },
    )

    if (messages.length > 0) {
      return messages
    }
  }

  if ('detail' in responseBody && typeof responseBody.detail === 'string') {
    return [responseBody.detail]
  }

  if ('title' in responseBody && typeof responseBody.title === 'string') {
    return [responseBody.title]
  }

  return [`Request failed with status ${response.status}.`]
}

function App() {
  const storesRequestId = useRef(0)
  const [stores, setStores] = useState<Store[] | null>(null)
  const [storesError, setStoresError] = useState<string | null>(null)
  const [isLoadingStores, setIsLoadingStores] = useState(true)
  const [isRefreshingStores, setIsRefreshingStores] = useState(false)
  const [brandId, setBrandId] = useState('')
  const [mallId, setMallId] = useState('')
  const [status, setStatus] = useState<InitialStoreStatus>(0)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [createErrors, setCreateErrors] = useState<string[]>([])
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    const requestId = ++storesRequestId.current

    async function loadInitialStores() {
      try {
        const loadedStores = await fetchStores(controller.signal)
        if (requestId === storesRequestId.current) {
          setStores(loadedStores)
          setStoresError(null)
        }
      } catch (loadError: unknown) {
        if (controller.signal.aborted || requestId !== storesRequestId.current) {
          return
        }

        setStoresError(getErrorMessage(loadError))
      } finally {
        if (!controller.signal.aborted && requestId === storesRequestId.current) {
          setIsLoadingStores(false)
        }
      }
    }

    void loadInitialStores()
    return () => controller.abort()
  }, [])

  async function refreshStores() {
    const requestId = ++storesRequestId.current
    setIsRefreshingStores(true)
    try {
      const refreshedStores = await fetchStores()
      if (requestId === storesRequestId.current) {
        setStores(refreshedStores)
        setStoresError(null)
      }
    } catch (loadError: unknown) {
      if (requestId === storesRequestId.current) {
        setStoresError(getErrorMessage(loadError))
      }
    } finally {
      if (requestId === storesRequestId.current) {
        setIsLoadingStores(false)
        setIsRefreshingStores(false)
      }
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (isSubmitting) {
      return
    }

    setIsSubmitting(true)
    setCreateErrors([])
    setSuccessMessage(null)

    const request: CreateStoreRequest = {
      brandId: brandId.trim(),
      mallId: mallId.trim(),
      status,
    }

    try {
      const response = await fetch(`${API_BASE_URL}/stores`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      })

      if (!response.ok) {
        setCreateErrors(await getApiErrorMessages(response))
        return
      }

      setSuccessMessage('Store created successfully.')
      setBrandId('')
      setMallId('')
      await refreshStores()
    } catch (submitError: unknown) {
      setCreateErrors([getErrorMessage(submitError)])
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="page">
      <div className="content">
        <header className="page-header">
          <p className="brand">ProjectNest</p>
          <h1>Stores</h1>
          <p className="subtitle">Store records across brands and malls.</p>
        </header>

        <section className="create-store" aria-labelledby="create-store-heading">
          <h2 id="create-store-heading">Create store</h2>
          <form onSubmit={handleSubmit} aria-busy={isSubmitting}>
            <div className="form-grid">
              <label className="form-field" htmlFor="brand-id">
                <span>Brand ID</span>
                <input
                  id="brand-id"
                  name="brandId"
                  type="text"
                  value={brandId}
                  onChange={(event) => setBrandId(event.target.value)}
                  placeholder="Brand UUID"
                  autoComplete="off"
                  required
                />
              </label>

              <label className="form-field" htmlFor="mall-id">
                <span>Mall ID</span>
                <input
                  id="mall-id"
                  name="mallId"
                  type="text"
                  value={mallId}
                  onChange={(event) => setMallId(event.target.value)}
                  placeholder="Mall UUID"
                  autoComplete="off"
                  required
                />
              </label>

              <label className="form-field" htmlFor="store-status">
                <span>Status</span>
                <select
                  id="store-status"
                  name="status"
                  value={status}
                  onChange={(event) =>
                    setStatus(event.target.value === '1' ? 1 : 0)
                  }
                >
                  <option value={0}>PreOpening</option>
                  <option value={1}>Open</option>
                </select>
              </label>
            </div>

            {createErrors.length > 0 && (
              <div className="form-message error" role="alert">
                <p>Store could not be created:</p>
                <ul>
                  {createErrors.map((message, index) => (
                    <li key={`${message}-${index}`}>{message}</li>
                  ))}
                </ul>
              </div>
            )}
            {successMessage !== null && (
              <p className="form-message success" role="status">
                {successMessage}
              </p>
            )}

            <button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Creating...' : 'Create store'}
            </button>
          </form>
        </section>

        <section className="store-list" aria-labelledby="store-list-heading">
          <h2 id="store-list-heading">All stores</h2>
          {isLoadingStores && (
            <p className="state" role="status">
              Loading stores...
            </p>
          )}
          {storesError !== null && (
            <p className="state error" role="alert">
              Unable to load stores: {storesError}
            </p>
          )}
          {isRefreshingStores && (
            <p className="state" role="status">
              Refreshing store list...
            </p>
          )}
          {!isLoadingStores && storesError === null && stores?.length === 0 && (
            <p className="state">No stores found.</p>
          )}
          {stores !== null && stores.length > 0 && (
            <div className="table-container">
              <table>
                <thead>
                  <tr>
                    <th scope="col">Store ID</th>
                    <th scope="col">Brand ID</th>
                    <th scope="col">Mall ID</th>
                    <th scope="col">Status</th>
                  </tr>
                </thead>
                <tbody>
                  {stores.map((store) => (
                    <tr key={store.id}>
                      <td>
                        <code>{store.id}</code>
                      </td>
                      <td>
                        <code>{store.brandId}</code>
                      </td>
                      <td>
                        <code>{store.mallId}</code>
                      </td>
                      <td>
                        <span className="status">
                          {statusLabels[store.status] ??
                            `Unknown (${store.status})`}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </div>
    </main>
  )
}

export default App
