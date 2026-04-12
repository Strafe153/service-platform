use serde::Deserialize;

#[derive(Deserialize)]
#[serde(rename_all = "PascalCase")]
pub struct MessageWrapper<T> {
    pub message: T,
}
