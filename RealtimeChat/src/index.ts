import "./css/main.css";
import { IMessageDTO, IPublicUserDTO } from "./dto/DTO";

let sessionList = document.getElementsByClassName("session-list")[0];
let chatContent = document.getElementsByClassName("chat-content")[0];
let currentSessionId = -1;
let currentSessionElement: HTMLLIElement;
let myUser: IPublicUserDTO;
let liList = sessionList.querySelectorAll("li");
let sendButton = <HTMLButtonElement>document.getElementById("send");
let messageField = <HTMLInputElement>document.getElementById("message-field");
loadMyUser();

messageField.addEventListener("keydown", async x => {
    if (x.key == 'Enter')
        await onMessageEnter();
});
sendButton.addEventListener("click", async x => {
    await onMessageEnter();
});
console.log(sessionList);

for (let i = 0; i < liList.length; i++) {
    let li = liList[i];
    li.addEventListener('click', x => {
        if (currentSessionElement && currentSessionElement.classList.contains("current"))
            currentSessionElement.classList.remove("current");
        currentSessionElement = li;
        currentSessionElement.classList.add("current");
        loadMessages(+li.querySelector("input").value);

    })
}
async function loadMessages(id: number) {
    currentSessionId = id;
    console.log(`session number ${id} loading...`);
    let messages: IMessageDTO[] = await getJson(`/chat/messages/${id}`);

    console.log(messages);
    chatContent.innerHTML = "";
    for (let m of messages) {
        chatContent.appendChild(createMessageElement(m));
    }

}
async function onMessageEnter() {
    let result = await postMessage(messageField.value);
    if (result) {
        messageField.value = "";
    }
}
async function postMessage(message: string): Promise<boolean> {
    if (currentSessionId < 0)
        return false;
    const response = await fetch("/chat/sendMessage?" + new URLSearchParams({
        message: message,
        sessionId: currentSessionId.toString()
    }).toString(), {
        method: 'POST',
        body: null,
        headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }
    });
    return response.ok;
}
async function getJson(url: string) {
    console.log(`request get to ${url}`);
    const response = await fetch(url, {
        method: 'GET',
        body: null,
        headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }
    });

    if (!response.ok)
        return null;
    if (response.body !== null)
        return await response.json();
    return null;
}
async function loadMyUser() {
    myUser = await getJson(`/account/myuser`);
    console.log(myUser);
}
function createMessageElement(message: IMessageDTO): HTMLDivElement {
    let msgDiv = div();
    msgDiv.classList.add("message");
    if (myUser.name == message.userName)
        msgDiv.classList.add("self");
    let textDiv = div();
    textDiv.classList.add("text");

    msgDiv.appendChild(textDiv);

    let nameText = document.createElement("h6");
    nameText.innerText = message.userName;
    let text = document.createElement("span");
    text.innerText = message.text;
    let date = document.createElement("span");
    date.classList.add("date");
    let d = new Date(message.date);
    date.innerText = ("0" + d.getHours()).slice(-2) + ":" + ("0" + d.getMinutes()).slice(-2);
    textDiv.appendChild(nameText);
    textDiv.appendChild(text);
    textDiv.appendChild(date);
    return msgDiv;
}
function div(): HTMLDivElement {
    return document.createElement('div');
}