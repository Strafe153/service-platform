use std::error::Error;

use async_trait::async_trait;
use uuid::Uuid;

use crate::{
    Page, dto::invoice_dto::InvoiceReadDto, repositories::invoice_repository::InvoiceRepository,
};

#[async_trait]
pub trait InvoiceService {
    async fn get(&self, page: Page) -> Result<Vec<InvoiceReadDto>, Box<dyn Error>>;
    async fn get_by_id(&self, id: Uuid) -> Result<InvoiceReadDto, Box<dyn Error>>;
}

pub struct InvoiceServiceImpl<R: InvoiceRepository> {
    pub repository: R,
}

impl<R: InvoiceRepository> InvoiceServiceImpl<R> {
    pub fn new(repository: R) -> Self {
        Self { repository }
    }
}

#[async_trait]
impl<R: InvoiceRepository + Sync> InvoiceService for InvoiceServiceImpl<R> {
    async fn get(&self, page: Page) -> Result<Vec<InvoiceReadDto>, Box<dyn Error>> {
        let skip = page.get_skip_count();
        let take = page.get_number();

        let dtos = self
            .repository
            .get(skip, take)
            .await?
            .into_iter()
            .map(|i| i.into())
            .collect();

        Ok(dtos)
    }

    async fn get_by_id(&self, id: Uuid) -> Result<InvoiceReadDto, Box<dyn Error>> {
        let dto = self.repository.get_by_id(id).await?.into();

        Ok(dto)
    }
}
