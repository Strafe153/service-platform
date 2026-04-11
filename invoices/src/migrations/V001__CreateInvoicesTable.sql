CREATE TABLE IF NOT EXISTS public.invoices
(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    orderid CHAR(24),
    createdAt TIMESTAMP
);