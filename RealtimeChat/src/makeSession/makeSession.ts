let userCount = 1;
let addUserButton = <HTMLButtonElement>document.getElementById("add-user");
let removeUserButton = <HTMLButtonElement>(
    document.getElementById("remove-user")
);
let usersGroup = <HTMLDivElement>(
    document.getElementsByClassName("users-group")[0]
);
addUserButton.addEventListener("click", (ev) => {
    userCount++;
    let userDiv = document.createElement("div");
    userDiv.classList.add("user", "number" + userCount);
    let label = document.createElement("label");
    let span = document.createElement("span");
    span.innerText = `Username(사용자 이름) ${userCount}:`;
    let input = document.createElement("input");
    input.name = `UserNames[${userCount - 1}]`;
    input.placeholder = "User name";
    label.appendChild(span);
    label.appendChild(input);
    userDiv.appendChild(label);
    usersGroup.appendChild(userDiv);
});
removeUserButton.addEventListener("click", (ev) => {
    if (userCount <= 1) return;
    usersGroup.removeChild(
        usersGroup.getElementsByClassName(`number${userCount}`)[0]
    );
    userCount--;
});
