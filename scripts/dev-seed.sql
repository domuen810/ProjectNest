-- Synthetic development data only. Safe to run repeatedly.
BEGIN;

INSERT INTO "Brands" ("Id", "Name", "Code", "IsActive", "CreatedAt", "UpdatedAt")
VALUES (
    'a1000000-0000-4000-8000-000000000001',
    'Brand A',
    'BRANDA',
    TRUE,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
)
ON CONFLICT DO NOTHING;

INSERT INTO "Malls" ("Id", "Name", "City", "Address", "IsActive", "CreatedAt", "UpdatedAt")
VALUES (
    'a2000000-0000-4000-8000-000000000001',
    'Mall A',
    'Sample City',
    '1 Example Street',
    TRUE,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
)
ON CONFLICT DO NOTHING;

INSERT INTO "Stores" ("Id", "BrandId", "MallId", "Status", "CreatedAt", "UpdatedAt")
SELECT
    'a3000000-0000-4000-8000-000000000001',
    brand."Id",
    mall."Id",
    0,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
FROM "Brands" AS brand
CROSS JOIN "Malls" AS mall
WHERE brand."NormalizedName" = 'brand a'
  AND mall."Name" = 'Mall A'
  AND mall."City" = 'Sample City'
  AND NOT EXISTS (
      SELECT 1
      FROM "Stores" AS existing_store
      WHERE existing_store."BrandId" = brand."Id"
        AND existing_store."MallId" = mall."Id"
  )
ON CONFLICT DO NOTHING;

COMMIT;
