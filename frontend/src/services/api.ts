import { authService } from "./authService";

export default async function apiFetch(url:string,options:RequestInit,contentType: string| undefined = undefined) {
  const token = await authService.getToken();

    const response = await fetch('/api' + url, {
        ...options,
        headers: {
            ...options?.headers,
           ...(contentType ? { "Content-Type": contentType } : {}),
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
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
