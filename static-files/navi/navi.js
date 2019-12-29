function submit() {
    let input = document.getElementById("naviInput");
    let main = document.getElementById("main");
    let url = "http://localhost:8080/navi";
    let options = {
        method: "POST",
        body: "street=" + input.value.trim()
    };

    fetch(url, options)
        .then(res => {
            if (res.status === 503) throw new Error("Plugin is busy");
            else if (!res.ok) throw new Error("Error");
            else return res.json()
        })
        .then(res => {
            M.toast({html: res.msg});
            let list = document.getElementById("list");
            if (list) main.removeChild(list);

            if (res.hasOwnProperty("cities") && res.cities.length > 0) {
                let ul = document.createElement("ul");
                ul.className = "collection";
                ul.id = "list";

                res.cities.forEach(city => {
                    let li = document.createElement("li");
                    li.className = "collection-item";
                    li.innerHTML = city;
                    ul.appendChild(li);
                });

                main.appendChild(ul);
            }
        })
        .catch(error => {
            M.toast({html: error.message});
        });
}

function reloadMap() {
    let url = "http://localhost:8080/navi?refresh=1";
    let options = {
        method: "GET"
    };

    fetch(url, options)
        .then(res => {
            if (res.status === 503) throw new Error("Plugin is busy");
            else if (!res.ok) throw new Error("Error");

            M.toast({html: 'Map is reloading...'});
        })
        .catch(error => {
            M.toast({html: error.message});
        });
}
