import useToastStore from "@/stores/useToastStore";



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
    console.error("API Error:", response.status, response.statusText);

    let errorCode = "generic";

    const contentType = response.headers.get("content-type");

    if (contentType?.includes("json")) {
      const data = await response.json();
      console.error("API Error JSON:", data);

      errorCode =  data.title || "generic";
    } 

    throw new Error(errorCode);
  }


  return response;
}
