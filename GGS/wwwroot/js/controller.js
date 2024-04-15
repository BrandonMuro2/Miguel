import { GetGames, GetList} from "./service.js";


$(async function () {
    LoadAll();
});



const LoadAll = async () => {
    ShowLoading();

    try {
        const responseGames= await GetGames();
        if (responseGames.Status != 200) {
            snackError(responseGames.Message);
            return;
        }


        const responseLists = await GetList();
        if (responseLists.Status != 200) {
            snackError(responseLists.Message);
            return;
        }
        console.log(responseGames.Data)

        console.log(responseLists.Data)

        renderGameCarousel(responseGames.Data)
        renderListCarousel(responseLists.Data)


    } catch (error) {
        console.error("An error occurred:", error);
        snackError('Ocurrió un error inesperado');
    } finally {
        HideLoading();
    }
}

function renderGameCarousel(data) {
    let items = '';
    let activeClass = ' active';

    for (let i = 0; i < data.length; i += 5) {
        let itemContent = `<div class="carousel-item${activeClass}"><div class="d-flex">`;

        for (let j = i; j < i + 5 && j < data.length; j++) {
            const pcRequirements = data[j].RequirementPcs.map(req => `${req.ReqType} CPU: ${req.ReqCpu}, RAM: ${req.ReqRam}GB, GPU: ${req.ReqCard}, Space: ${req.ReqSpace}GB`).join('<br>');
            const consoleRequirements = data[j].RequirementConsoles.map(req => `${req.ConName} Espacio: ${req.ConSpace}GB`).join('<br>');
            const stars = generateRatingStars(data[j].GameRating);

            let platformIconHTML = '';
            if (data[j].RequirementPcs.length > 0) {
                platformIconHTML += '<i class="mx-1 fas fa-desktop"></i>'; // Icono para PC
            }
            data[j].RequirementConsoles.forEach(req => {
                if (req.ConName.includes("Xbox")) {
                    platformIconHTML += '<i class="mx-1 fab fa-xbox" style="color: #107c10;"></i>'; // Icono verde para Xbox
                } else if (req.ConName.includes("PlayStation 5")) {
                    platformIconHTML += '<i class="mx-1 fab fa-playstation" style="color: #003791;"></i>'; // Icono azul para PlayStation
                }
            });

            itemContent += `
                <div class="card bg-dark" style="margin: 10px; width: 18rem; height: auto;">
                    <img src="${data[j].GameImg}" class="card-img-top" alt="${data[j].GameName}" style="height: 400px; object-fit: cover;">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center">
                            <h5 class="card-title">${data[j].GameName}</h5>
                            <span>${platformIconHTML}</span>
                        </div>
                        <p class="card-text">${data[j].GameDescription}</p>
                        <div>${stars}</div>
                    </div>
                    <div class="card-hover-info">
                        <div class="card-img-overlay">`;
            if (data[j].RequirementPcs.length > 0) {
                itemContent += `<strong>Requisitos de PC:</strong><br>${pcRequirements}<br><br>`;
            }
            if (data[j].RequirementConsoles.length > 0) {
                itemContent += `<strong>Requisitos de Consola:</strong><br>${consoleRequirements}`;
            }
            itemContent += `</div>
                    </div>
                </div>`;
        }

        itemContent += '</div></div>';
        items += itemContent;
        activeClass = ''; // Solo el primer elemento debe tener la clase 'active'
    }

    $('#gameCarousel .carousel-inner').html(items);
}



function generateRatingStars(rating) {
    let stars = '';
    for (let k = 1; k <= 5; k++) {
        if (k <= rating) {
            stars += '<i class="fas fa-star" style="color: gold;"></i>'; // Estrella llena
        } else {
            stars += '<i class="far fa-star" style="color: gold;"></i>'; // Estrella vacía
        }
    }
    return stars;
}


function renderListCarousel(listData) {
    const $carouselInner = $('#listCarousel .carousel-inner').empty();
    let groupCount = 0; // Contador para agrupar las listas

    // Itera sobre las listas y agrupa cada 5 listas en un 'carousel-item'
    for (let i = 0; i < listData.length; i += 5) {
        const group = listData.slice(i, i + 5); // Obtén el grupo actual de hasta 5 listas
        let carouselItemContent = group.map(list => {
            let firstGame = list.Games[0] || {}; // Asegúrate de que al menos hay un juego
            let daysSinceCreated = calculateDaysSince(new Date(list.ListCreatedAt)); // Calcula los días desde la creación
            return `
                <div class="card bg-dark text-white" style="margin: 10px;width: 18rem; height: auto;">
                    <img src="${firstGame.GameImg || 'url_to_default_image_if_needed.jpg'}" class="card-img-top" alt="${firstGame.GameName || 'No image available'}">
                    <div class="card-body">
                        <h5 class="card-title">${list.ListName}</h5>
                        <p class="card-text">${list.ListDesc}</p>
                        <span class="badge bg-secondary text-light">${daysSinceCreated} días </span>
                        <p class="mt-3 card-text"> ${list.Us.UsName}</p>
                    </div>
                </div>
            `;
        }).join(''); // Une todos los elementos del grupo en una cadena de HTML

        // Envuelve el grupo en un elemento 'carousel-item'
        let carouselItem = $(`
            <div class="carousel-item ${groupCount === 0 ? 'active' : ''}">
                <div class="d-flex justify-content-around">${carouselItemContent}</div>
            </div>
        `);

        $carouselInner.append(carouselItem); // Añade el grupo al carrusel
        groupCount++; // Incrementa el contador de grupos
    }
}

// Calcula los días desde una fecha dada hasta hoy
function calculateDaysSince(date) {
    const today = new Date();
    const timeDiff = today - date; // Diferencia en milisegundos
    return Math.floor(timeDiff / (1000 * 3600 * 24)); // Convierte milisegundos a días
}



function ShowLoading() {
    $.blockUI({
        message:
            `<div class="card border border-0 my-3 py-3" style="background-color: #333; color: #fff;">
                <div class="text-center mb-2">
                    <div class="spinner-border text-light" style="width: 3rem; height: 3rem;" role="status">
                        <span class="visually-hidden">Cargando...</span>
                    </div>
                </div>
                <h6 class="card-text fw-bold">Cargando contenido, por favor espere...</h6>
            </div>`,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#333', /* fondo más oscuro */
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            borderRadius: '10px',
            opacity: .9,
            color: '#fff' /* texto en blanco */
        }
    });
}

function HideLoading() {
    $.unblockUI();
}


