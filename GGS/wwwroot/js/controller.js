import { GetActivityByChannels, GetChannels, GetSignalsBySubject, GetSubjects, InsertBackgroundAndTask } from "./service.js";

const channelZones = {
    "EEG Fp1": "Frontal",
    "EEG Fp2": "Frontal",
    "EEG F3": "Frontal",
    "EEG F4": "Frontal",
    "EEG F7": "Frontal",
    "EEG F8": "Frontal",
    "EEG T3": "Temporal",
    "EEG T4": "Temporal",
    "EEG C3": "Central",
    "EEG C4": "Central",
    "EEG T5": "Temporal",
    "EEG T6": "Temporal",
    "EEG P3": "Parietal",
    "EEG P4": "Parietal",
    "EEG O1": "Occipital",
    "EEG O2": "Occipital",
    "EEG Fz": "Frontal",
    "EEG Cz": "Central",
    "EEG Pz": "Parietal",
    "EEG A2-A1": "Reference",
    "ECG ECG": "Heart"
};

$(async function () {
    LoadAll();
});

const LoadAll = async () => {
    ShowLoading();

    try {
        const [responseSubjects, responseChannels] = await Promise.all([
            GetSubjects(),
            GetChannels()
        ]);

        if (responseSubjects.Status != 200) {
            snackError(responseSubjects.Message);
            return;
        }
        if (responseChannels.Status != 200) {
            snackError(responseChannels.Message);
            return;
        }


        populateSubjects(responseSubjects.Data);
        populateChannels(responseChannels.Data);

    } catch (error) {
        console.error("An error occurred:", error);
    } finally {
        HideLoading();
    }
}

const populateSubjects = (subjects) => {
    const subjectSelect = $('#subjectSelect');
    subjectSelect.empty()
    subjectSelect.append(new Option(`Select a subject`, ""));



    subjects.forEach(subject => {
        subjectSelect.append(new Option(`Subject ${subject.SubjectId}`, subject.SubjectId));
    });

    subjectSelect.on('change', function () {
        const selectedSubject = subjects.find(subject => subject.SubjectId == this.value);
        $('#subjectAge').val(selectedSubject.Age);
        $('#subjectGender').val(selectedSubject.Gender);

        // Trigger search if a channel is already selected
        const selectedChannel = $('#channelList .active').data('channel-id');
        if (selectedChannel) {
            getEEGBySubjectChannel(selectedChannel);
        }
    });
};

const populateChannels = (channels) => {
    const channelList = $('#channelList');
    channelList.empty();
    channels.forEach(channel => {
        const zone = channelZones[channel.ChannelName] || "Unknown";
        channelList.append(`<li class="list-group-item bg-dark text-light border border-white" data-channel-id="${channel.ChannelId}" data-zone="${zone}" data-name="${channel.ChannelName}">${channel.ChannelName} (${zone})</li>`);
    });

    // Añadimos el event listener a los elementos de la lista de canales
    $('#channelList').on('click', '.list-group-item', function () {
        $('#channelList .list-group-item').removeClass('active');
        $(this).addClass('active');
        const channelId = $(this).data('channel-id');
        const zoneName = $(this).data('zone');
        const channelName = $(this).data('name');

        $('#zoneName').text("Average global activity: " + zoneName);
        $('#channelName').text("Average subject activity: "+channelName);

        getEEGBySubjectChannel(channelId);
        const channelsInZone = Object.keys(channelZones).filter(name => channelZones[name] === zoneName);
        const channelsString = channelsInZone.join(',');
        calculateGlobalActivityDifference(channelsString);
    });
};

const calculateActivityDifference = (backgroundData, taskData) => {
    const backgroundAmplitude = backgroundData.reduce((sum, d) => sum + Math.abs(d.Amplitude), 0) / backgroundData.length;
    const taskAmplitude = taskData.reduce((sum, d) => sum + Math.abs(d.Amplitude), 0) / taskData.length;
    return taskAmplitude - backgroundAmplitude;
};

const calculateGlobalActivityDifference = async (channelNames) => {
    const subjectId = $('#subjectSelect').val();
    if (!subjectId) {
        snackError('Please select a subject first.');
        return;
    }

    ShowLoading();

    try {
        const responseAll = await GetActivityByChannels(channelNames);

        if (responseAll.Status !== 200) {
            snackError('Error fetching data.');
            return;
        }

        const activityData = responseAll.Data;

        // Separate Task and Background data
        const taskData = activityData.filter(d => d.SessionType === 'Task');
        const backgroundData = activityData.filter(d => d.SessionType === 'Background');

        // Ensure we have matching task and background data for each channel
        const differences = taskData.map(task => {
            const background = backgroundData.find(bg => bg.ChannelId === task.ChannelId);
            if (background) {
                return {
                    channelId: task.ChannelId,
                    difference: task.AvgAmplitude - background.AvgAmplitude
                };
            }
            return null;
        }).filter(d => d !== null);

        // Calculate the global average difference
        if (differences.length === 0) {
            snackError('No matching background data found for the selected channels.');
            return;
        }

        const globalDifference = differences.reduce((sum, diff) => sum + diff.difference, 0) / differences.length;

        $('#zoneInfo').text(`Extra activity: ${globalDifference.toFixed(2)} µV`);
        plotZoneSummaryChart(backgroundData, taskData, 'eegPlotZoneSummary', 'Zone Summary', zoneName);

    } catch (error) {
        console.error("An error occurred:", error);
    } finally {
        HideLoading();
    }
};


const getEEGBySubjectChannel = async (channelId) => {
    const subjectId = $('#subjectSelect').val();
    if (!subjectId) {
        snackError('Please select a subject first.');
        return;
    }

    ShowLoading();

    try {
        const [responseBackground, responseTask] = await Promise.all([
            GetSignalsBySubject(subjectId, channelId, 'Background'),
            GetSignalsBySubject(subjectId, channelId, 'Task')
        ]);

        if (responseBackground.Status !== 200) {
            snackError(responseBackground.Message);
            return;
        }
        if (responseTask.Status !== 200) {
            snackError(responseTask.Message);
            return;
        }

        processAndPlotData(responseBackground.Data, 'eegPlotBack', 'EEG Data - Normal', '#32CD32'); // Verde lima
        processAndPlotData(responseTask.Data, 'eegPlotTask', 'EEG Data - Performing operations', '#DA70D6'); // Orquídea

        const activityDifference = calculateActivityDifference(responseBackground.Data, responseTask.Data);
        $('#activityInfo').text(`Extra activity: ${activityDifference.toFixed(2)} µV`);

        plotBarChart(responseBackground.Data, responseTask.Data, 'eegPlotChannelSummary', 'Channel Summary Activity');



    } catch (error) {
        console.error("An error occurred:", error);
    } finally {
        HideLoading();
    }
};

const processAndPlotData = (data, plot,title, lineColor) => {
    const originalSampleFrequency = 500; // Frecuencia de muestreo original en Hz
    const downSampleFactor = 10; // Factor de muestreo (reducción a 10 Hz)
    const sampleFrequency = originalSampleFrequency / downSampleFactor; // Nueva frecuencia de muestreo en Hz

    // Convertir índices de tiempo a segundos basados en la frecuencia original
    const timeStamps = data.map(d => d.TimeIndex / originalSampleFrequency);
    const amplitudes = data.map(d => d.Amplitude);

    const trace = {
        x: timeStamps,
        y: amplitudes,
        mode: 'lines',
        line: { color: lineColor }, 
        name: 'EEG Channel'
    };

    const layout = {
        title: title, // Título del gráfico
        paper_bgcolor: '#1f1f1f', // Color de fondo del papel
        plot_bgcolor: '#1f1f1f',  // Color de fondo del gráfico
        font: {
            color: '#ffffff'      // Color del texto
        },
        xaxis: {
            title: 'Time (s)',
            color: '#ffffff'      // Color de las etiquetas del eje x
        },
        yaxis: {
            title: 'Amplitude (µV)',
            color: '#ffffff'      // Color de las etiquetas del eje y
        }
    };

    Plotly.newPlot(plot, [trace], layout);
};






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


$(document).on("click", "#uploadButton", async function () {
    var formData = new FormData();
    var fileBackground = $('#fileBackground')[0].files[0];
    var fileTask = $('#fileTask')[0].files[0];

    formData.append('backgroundfile', fileBackground);
    formData.append('taskfile', fileTask);

    ShowLoading();

    try {

        var response = await InsertBackgroundAndTask(formData);

        if (response.Status !== 200) {
            snackError(response.Message);
            return;
        }




    } catch (error) {
        console.error("An error occurred:", error);
    } finally {
        HideLoading();
    }
});

const plotBarChart = (backgroundData, taskData, plotId, title) => {
    const backgroundAvg = backgroundData.map(d => Math.abs(d.Amplitude));
    const taskAvg = taskData.map(d => Math.abs(d.Amplitude));

    const trace1 = {
        x: ['Background', 'Task'],
        y: [backgroundAvg.reduce((sum, value) => sum + value, 0) / backgroundAvg.length,
        taskAvg.reduce((sum, value) => sum + value, 0) / taskAvg.length],
        type: 'bar',
        name: 'Activity'
    };

    const layout = {
        paper_bgcolor: '#1f1f1f', // Color de fondo del papel
        plot_bgcolor: '#1f1f1f',  // Color de fondo del gráfico
        font: {
            color: '#ffffff'      // Color del texto
        },
        xaxis: {
            color: '#ffffff'      // Color de las etiquetas del eje x
        },
        yaxis: {
            title: 'Amplitude (µV)',
            color: '#ffffff'      // Color de las etiquetas del eje y
        }
    };

    Plotly.newPlot(plotId, [trace1], layout);
};

const plotZoneSummaryChart = (backgroundData, taskData, plotId, title, zoneName) => {
    const backgroundAvg = backgroundData.reduce((sum, d) => sum + d.AvgAmplitude, 0) / backgroundData.length;
    const taskAvg = taskData.reduce((sum, d) => sum + d.AvgAmplitude, 0) / taskData.length;

    const trace1 = {
        x: ['Background', 'Task'],
        y: [backgroundAvg, taskAvg],
        type: 'bar',
        name: 'Activity'
    };

    const layout = {
        paper_bgcolor: '#1f1f1f', // Color de fondo del papel
        plot_bgcolor: '#1f1f1f',  // Color de fondo del gráfico
        font: {
            color: '#ffffff'      // Color del texto
        },
        xaxis: {
            title: 'Activity Type',
            color: '#ffffff'      // Color de las etiquetas del eje x
        },
        yaxis: {
            title: 'Average Amplitude (µV)',
            color: '#ffffff'      // Color de las etiquetas del eje y
        }
    };

    Plotly.newPlot(plotId, [trace1], layout);
};