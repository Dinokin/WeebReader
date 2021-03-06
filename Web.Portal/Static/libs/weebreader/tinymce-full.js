﻿tinymce.init({
    language: langCode === "en_US" ? null : langCode,
    selector: "textarea",
    plugins: "autosave wordcount link image paste",
    toolbar: "restoredraft wordcount link image paste",
    height: 500,
    paste_data_images: true,
    file_picker_types: "image",
    file_picker_callback: (cb, value, meta) => {
        let input = document.createElement("input");

        input.setAttribute("type", "file");
        input.setAttribute("accept", "image/*");

        input.onchange = () => {
            let file = input.files[0];
            let reader = new FileReader();

            reader.onload = () => {
                let id = "blobid" + (new Date()).getTime();
                let blobCache = tinymce.activeEditor.editorUpload.blobCache;
                let base64 = reader.result.split(",")[1];
                let blobInfo = blobCache.create(id, file, base64);
                blobCache.add(blobInfo);

                cb(blobInfo.blobUri(), { title: file.name });
            };

            reader.readAsDataURL(file);
        };

        input.click();
    }
});