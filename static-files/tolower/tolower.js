function submit() {
    let textarea = document.getElementById("toLowerTextarea");
    let form = document.getElementById("toLowerForm");
    let url = "http://localhost:8080/tolower";
    let options = {
        method: "POST",
        body: "text=" + textarea.value.trim()
    };

    fetch(url, options)
        .then(res => res.text())
        .then(res => {
            let pre = document.createElement("pre");
            pre.innerHTML = res;
            pre.classList.add("white-text");
            form.append(pre);
            textarea.value = "";
            M.textareaAutoResize(textarea);
        });
}