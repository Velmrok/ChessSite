import useUserStore from "@/stores/useUserStore";

const API_URL = import.meta.env.VITE_API_URL
export default async function apiFetch(url: string, method:string, verifyToken: boolean=false,
    contentType?:string,body?: any ) {
        const headers = contentType? {
            "Content-Type": contentType,
        } : undefined;
  const response = await fetch(`${API_URL}${url}`, {
    method: method,
   headers: headers,
    credentials: verifyToken ? "include" : "same-origin",
    body: body ? body : null,
  });
    if (!response.ok) {

        const errorData = await response.json();
        throw new Error(errorData.message);
    }
 

  return response;
}