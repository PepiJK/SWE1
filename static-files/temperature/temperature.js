function getTemps(date = 'null', pageindex = 1, pagesize = 10) {
    let url = "http://localhost:8080/temperature/json";
    let options = { method: "GET" };

    if (date != 'null') url = url + "/" + date;
    url = url + "?pageindex=" + pageindex + "&pagesize=" + pagesize;

    console.log(url);

    fetch(url, options)
        .then(res => res.json())
        .then(res => {
            // remove every entry in the table to load the new data
            let tableBody = document.getElementById("tableBody");
            tableBody.innerHTML = "";

            // populate table
            res.Items.forEach(item => {
                let tr = document.createElement("tr");

                let tdId = document.createElement("td");
                tdId.innerHTML = item.Id;
                let tdDateTime = document.createElement("td");
                tdDateTime.innerHTML = new Date(item.DateTime).toLocaleString("de-AT")
                let tdValue = document.createElement("td");
                tdValue.innerHTML = item.Value;

                tr.appendChild(tdId);
                tr.appendChild(tdDateTime);
                tr.appendChild(tdValue);

                tableBody.appendChild(tr);
            });

            // setup paginator
            let paginator = document.getElementById("paginator");
            paginator.innerHTML = "";

            if (res.TotalPages > 1) {
                if (res.PageIndex == 1) paginator.innerHTML += `<li class="disabled"><a><i class="material-icons">chevron_left</i></a></li>`;
                else {
                    paginator.innerHTML += `<li><a onclick="getTemps('${date}', 1, ${pagesize})"><i class="material-icons">first_page</i></a></li>`;
                    paginator.innerHTML += `<li><a onclick="getTemps('${date}', ${pageindex - 1}, ${pagesize})"><i class="material-icons">chevron_left</i></a></li>`;
                }

                switch (res.PageIndex) {
                    case 1:
                        paginator.innerHTML += `<li class="active"><a>${pageindex}</a></li>`;
                        paginator.innerHTML += `<li class="waves-effect"><a onclick="getTemps('${date}', ${pageindex + 1}, ${pagesize})">${pageindex + 1}</a></li>`;
                        break;
                    case res.TotalPages:
                        paginator.innerHTML += `<li class="waves-effect"><a onclick="getTemps('${date}', ${pageindex - 1}, ${pagesize})">${pageindex - 1}</a></li>`;
                        paginator.innerHTML += `<li class="active"><a>${pageindex}</a></li>`;
                        break;

                    default:
                        paginator.innerHTML += `<li class="waves-effect"><a onclick="getTemps('${date}', ${pageindex - 1}, ${pagesize})">${pageindex - 1}</a></li>`;
                        paginator.innerHTML += `<li class="active"><a>${pageindex}</a></li>`;
                        paginator.innerHTML += `<li class="waves-effect"><a onclick="getTemps('${date}', ${pageindex + 1}, ${pagesize})">${pageindex + 1}</a></li>`;
                        break;
                }

                if (res.PageIndex == res.TotalPages) paginator.innerHTML += `<li class="disabled"><a><i class="material-icons">chevron_right</i></a></li>`;
                else {
                    paginator.innerHTML += `<li><a onclick="getTemps('${date}', ${pageindex + 1}, ${pagesize})"><i class="material-icons">chevron_right</i></a></li>`;
                    paginator.innerHTML += `<li><a onclick="getTemps('${date}', ${res.TotalPages}, ${pagesize})"><i class="material-icons">last_page</i></a></li>`;
                }
            }
        });

}

function submitDate() {
    let elem = document.getElementById("datePicker");
    let instance = M.Datepicker.getInstance(elem);
    let date = instance.date.getFullYear() + "-" + ("0" + (instance.date.getMonth() + 1)).slice(-2) + "-" + ("0" + instance.date.getDate()).slice(-2);
    getTemps(date, 1);
}

document.addEventListener('DOMContentLoaded', function () {
    var options = {
        format: "dd.m.yyyy",
    }
    let elem = document.getElementById("datePicker");
    M.Datepicker.init(elem, options);

    getTemps('null', 1);
});


