use std::sync::Arc;

use crate::services::invoice_service::InvoiceService;

pub struct AppState {
    pub service: Arc<dyn InvoiceService>,
}

impl AppState {
    pub fn new(service: Arc<dyn InvoiceService>) -> Self {
        Self { service }
    }
}
