import useUserStore from "@/stores/useUserStore";

const API_URL = import.meta.env.VITE_API_URL
export default async function apiFetch(url: string, method:string, verifyToken: boolean=false,
    contentType?:string,body?: any ) {
        const headers = contentType? {
            "Content-Type": contentType,
        } : undefined;
  const response = await fetch(`/api${url}`, {
    method: method,
    headers: headers,
    credentials: verifyToken ? "include" : "same-origin",
    body: body ? body : null,
  });
  if (!response.ok) {
    const data = await response.json();
    console.error("API Error:", data);
    const errorCode = data.title || "generic";
    const error = new Error(errorCode);

    throw error;
  }


  return response;
}
