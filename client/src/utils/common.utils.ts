const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isGuid(value: string | undefined): boolean {
  if (!value) {
    return false;
  }

  return guidRegex.test(value);
}