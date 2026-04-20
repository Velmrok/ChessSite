
export default async function apiFetch(options:{
  url: string,
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH',
  includeCredentials?: boolean,
  contentType?: string,
  body?: any}) {
  const headers = options.contentType ? {
    "Content-Type": options.contentType,
  } : undefined;
  const response = await fetch(`/api${options.url}`, {
    method: options.method,
    headers: headers,
    credentials: options.includeCredentials ? "include" : "same-origin",
    body: options.body ? options.body : null,
  });

  if (!response.ok) {
    console.error("API Error:", response.status, response.statusText);

    let errorCode = "generic";

    const responseContentType = response.headers.get("content-type");

    if (responseContentType?.includes("json")) {
      const data = await response.json();
      console.error("API Error Details:", data);
      errorCode = data.title || "generic";
    }

    throw Object.assign(new Error(errorCode), { status: response.status });
  }

  if (response.status === 204 || response.headers.get("content-length") === "0") {
    return response;
  }

  return await response.json();
}
