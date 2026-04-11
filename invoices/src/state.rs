use crate::services::invoice_service::InvoiceService;

pub struct AppState<S: InvoiceService> {
    pub service: S,
}

impl<S: InvoiceService> AppState<S> {
    pub fn new(service: S) -> Self {
        Self { service }
    }
}
