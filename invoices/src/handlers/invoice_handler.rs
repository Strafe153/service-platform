use std::sync::Arc;

use axum::{
    Json,
    extract::{Path, Query, State},
    http::StatusCode,
    response::{IntoResponse, Response},
};
use uuid::Uuid;

use crate::{
    AppState,
    models::page::Page,
    repositories::invoice_repository::PostgresInvoiceRepository,
    services::invoice_service::{InvoiceService, InvoiceServiceImpl},
};

pub async fn get_by_id(
    State(state): State<Arc<AppState<InvoiceServiceImpl<PostgresInvoiceRepository>>>>,
    Path(id): Path<Uuid>,
) -> Response {
    let result = state.service.get_by_id(id).await;

    match result {
        Ok(invoice) => (StatusCode::OK, Json(invoice)).into_response(),
        Err(_) => StatusCode::NOT_FOUND.into_response(),
    }
}

pub async fn get_page(
    State(state): State<Arc<AppState<InvoiceServiceImpl<PostgresInvoiceRepository>>>>,
    Query(page): Query<Page>,
) -> Response {
    let result = state.service.get(page).await;

    match result {
        Ok(invoices) => (StatusCode::OK, Json(invoices)).into_response(),
        Err(_) => StatusCode::BAD_REQUEST.into_response(),
    }
}
