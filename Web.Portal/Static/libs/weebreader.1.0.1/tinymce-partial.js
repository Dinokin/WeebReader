tinymce.init({
    language: langCode === "en_US" ? null : langCode, 
    selector: "textarea",
    plugins: "autosave wordcount link paste",
    toolbar: "restoredraft wordcount link paste",
    height: 500
});