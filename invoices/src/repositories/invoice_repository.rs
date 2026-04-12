use async_trait::async_trait;
use std::error::Error;
use tokio_postgres::Client;
use uuid::Uuid;

use crate::{dto::invoice_dto::InvoiceCreateDto, entities::invoice::Invoice};

#[async_trait]
pub trait InvoiceRepository: Send + Sync {
    async fn get(&self, skip: i64, take: i64) -> Result<Vec<Invoice>, Box<dyn Error>>;
    async fn get_by_id(&self, id: Uuid) -> Result<Invoice, Box<dyn Error>>;
    async fn create(&self, dto: InvoiceCreateDto) -> Result<(), Box<dyn Error>>;
}

pub struct PostgresInvoiceRepository {
    pub client: Client,
}

#[async_trait]
impl InvoiceRepository for PostgresInvoiceRepository {
    async fn get(&self, skip: i64, take: i64) -> Result<Vec<Invoice>, Box<dyn Error>> {
        let rows = self
            .client
            .query(
                "SELECT id, orderId, createdAt FROM invoices LIMIT $1 OFFSET $2",
                &[&take, &skip],
            )
            .await?;

        let invoices: Vec<Invoice> = rows
            .iter()
            .map(|r| Invoice::new(r.get(0), r.get(1), r.get(2)))
            .collect();

        Ok(invoices)
    }

    async fn get_by_id(&self, id: Uuid) -> Result<Invoice, Box<dyn Error>> {
        let row = self
            .client
            .query_one(
                "SELECT id, orderId, createdAt FROM invoices WHERE id = $1",
                &[&id],
            )
            .await?;

        let invoice = Invoice::new(row.get(0), row.get(1), row.get(2));

        Ok(invoice)
    }

    async fn create(&self, dto: InvoiceCreateDto) -> Result<(), Box<dyn Error>> {
        self.client
            .execute(
                "INSERT INTO invoices(orderId, createdAt) VALUES ($1, $2)",
                &[&dto.order_id, &dto.created_at],
            )
            .await?;

        Ok(())
    }
}
