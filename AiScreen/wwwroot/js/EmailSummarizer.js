function openSummarizerDialog(event) {
    Office.context.ui.displayDialogAsync(
        "https://azhamrosli.com/Home/EmailSummarize",
        {
            height: 70,
            width: 60,
            displayInIframe: true
        },
        function (asyncResult) {
            if (asyncResult.status === Office.AsyncResultStatus.Succeeded) {
                const dialog = asyncResult.value;
            }
        }
    );
}