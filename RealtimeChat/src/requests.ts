export async function getJson(url: string) {
    console.log(`request get to ${url}`);
    const response = await fetch(url, {
        method: "GET",
        body: null,
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
        },
    });

    if (!response.ok) return null;
    if (response.body !== null) return await response.json();
    return null;
}
export async function post(url: string) {
    console.log(`request post to ${url}`);
    const response = await fetch(url, {
        method: "POST",
        body: null,
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
        },
    });
    return response.ok;
}
