export function GetList() {
    return $.ajax({
        method: 'POST',
        url:  '/Home/GetList',
        dataType: 'json',

    })
}
export function GetGames() {
    return $.ajax({
        method: 'POST',
        url: '/Home/GetGames',
        dataType: 'json',

    })
}
