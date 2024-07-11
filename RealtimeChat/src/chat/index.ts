import "../css/main.css";
import "../css/main.scss";
import { IChatSessionDTO, IMessageDTO, IPublicUserDTO } from "../dto/DTO";
import { elements } from "./elements";
import { myUser, loadMyUser } from "./myUser";
import { getJson, post } from "./requests";
import "./srListener";
import { globalState } from "./state";

loadMyUser();

elements.messageField.addEventListener("keydown", async (x) => {
    if (x.key == "Enter") await onMessageEnter();
});
elements.sendButton.addEventListener("click", async (x) => {
    await onMessageEnter();
});
console.log(elements.sessionList);

for (let i = 0; i < elements.liList.length; i++) {
    let li = elements.liList[i];
    li.addEventListener("click", (x) => {
        if (
            elements.currentSessionElement &&
            elements.currentSessionElement.classList.contains("current")
        )
            elements.currentSessionElement.classList.remove("current");
        elements.currentSessionElement = li;
        elements.currentSessionElement.classList.add("current");
        globalState.currentSessionId = +li.querySelector("input").value;
        loadMessages(globalState.currentSessionId);
        loadSession(globalState.currentSessionId);
    });
}
async function loadSession(id: number) {
    console.log(`session number ${id} loading...`);
    let sessions: IChatSessionDTO = await getJson(`/chat/sessionInfo/${id}`);

    console.log(sessions);
    elements.hUserNames.innerText = sessions.users
        .map((x) => x.name)
        .join(", ");
}
async function loadMessages(id: number) {
    console.log(`messages of session number ${id} loading...`);
    let messages: IMessageDTO[] = await getJson(`/chat/messages/${id}`);

    console.log(messages);
    elements.chatContent.innerHTML = "";
    for (let m of messages) {
        elements.chatContent.appendChild(
            elements.createMessageElement(m, myUser.name == m.userName)
        );
    }
}
async function onMessageEnter() {
    let message = elements.messageField.value;
    if (message == null || message.trim() === '')
        return;
    let result = await postMessage(message);
    if (result) {
        elements.messageField.value = "";
    }
}
async function postMessage(message: string): Promise<boolean> {
    if (globalState.currentSessionId < 0) return false;
    return await post(
        "/chat/sendMessage?" +
            new URLSearchParams({
                message: message,
                sessionId: globalState.currentSessionId.toString(),
            }).toString()
    );
}
