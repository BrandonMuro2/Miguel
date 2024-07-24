
export function GetSignalsBySubject(subjectId, channelId, type) {
    return $.ajax({
        method: 'GET',
        url: '/Home/GetSignalsBySubject',
        dataType: 'json',
        data: {
            subjectId: subjectId,
            channelId: channelId,
            type: type
        }


    })
}

export function GetSubjects() {
    return $.ajax({
        method: 'GET',
        url: '/Home/GetSubjects',
        dataType: 'json',
    })
}


export function GetChannels() {
    return $.ajax({
        method: 'GET',
        url: '/Home/GetChannels',
        dataType: 'json',
    })
}
export function GetActivityByChannels(channelNames) {
    return $.ajax({
        method: 'GET',
        url: '/Home/GetActivityByChannels',
        dataType: 'json',
        data: {
            channelNames: channelNames
        }
    })
}

export function InsertBackgroundAndTask(formData) {
    return $.ajax({
        method: 'POST',
        url: '/Home/InsertBackgroundAndTask',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,

    })
}

