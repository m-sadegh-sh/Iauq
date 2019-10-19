using System.Web.Mvc;

namespace Iauq.Information.Areas.Administration
{
    public class AdministrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Administration"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("AdministrationDefault", "administration/",
                             new {Controller = "Administration", Action = "Default"});

            context.MapRoute("AdministrationChangePassword", "administration/change-password",
                             new {Controller = "Administration", Action = "ChangePassword"});

            context.MapRoute("AdministrationComments", "administration/comments/owned-by/{page}/{OwnerId}/",
                             new {Controller = "Comments", Action = "List"},
                             new {page = @"\d+", OwnerId = @"\d+"});

            context.MapRoute("AdministrationContentsList", "administration/contents/{ContentType}/{page}",
                             new {Controller = "Contents", Action = "List"},
                             new {page = @"\d+"});

            context.MapRoute("AdministrationLogsList", "administration/logs/{logLevel}/{page}",
                             new {Controller = "Logs", Action = "List"},
                             new {page = @"\d+"});

            context.MapRoute("AdministrationList", "administration/{Controller}/{page}",
                             new {Action = "List"},
                             new {page = @"\d+"});

            context.MapRoute("AdministrationContents", "administration/{Controller}/{ContentType}/create/",
                             new {Action = "Create"});

            context.MapRoute("AdministrationCreate", "administration/{Controller}/create/", new {Action = "Create"});

            context.MapRoute("AdministrationEdit", "administration/{Controller}/{Id}/edit",
                             new {Action = "Edit"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationDetails", "administration/{Controller}/{Id}/details",
                             new {Action = "Details"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationDelete", "administration/{Controller}/{Id}/delete/",
                             new {Action = "Delete"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationChangeApproval", "administration/comments/{Id}/change-approval/",
                             new {Controller = "Comments", Action = "ChangeApproval"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationChangePublication", "administration/{controller}/{Id}/change-publication/",
                             new {Action = "ChangePublication"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationCreateChoice", "administration/{Controller}/{pollId}/create-choice/",
                             new {Action = "CreateChoice"},
                             new {pollId = @"\d+"});

            context.MapRoute("AdministrationEditChoice", "administration/{Controller}/{Id}/edit-choice",
                             new {Action = "EditChoice"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationDeleteChoice", "administration/{Controller}/{Id}/delete-choice/",
                             new {Action = "DeleteChoice"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationCreateChoiceItem",
                             "administration/{Controller}/{choiceId}/create-choice-item/",
                             new {Action = "CreateChoiceItem"},
                             new {choiceId = @"\d+"});

            context.MapRoute("AdministrationEditChoiceItem", "administration/{Controller}/{Id}/edit-choice-item",
                             new {Action = "EditChoiceItem"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationDeleteChoiceItem", "administration/{Controller}/{Id}/delete-choice-item/",
                             new {Action = "DeleteChoiceItem"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationImagesList", "administration/images/",
                             new {Controller = "Images", Action = "List"});

            context.MapRoute("AdministrationImagesUpload", "administration/images/upload/",
                             new {Controller = "Images", Action = "Upload"});

            context.MapRoute("AdministrationFileManagerCreateDirectory",
                             "administration/file-manager/create-directory/{*targetUrl}",
                             new {Controller = "FileManager", Action = "CreateDirectory"});

            context.MapRoute("AdministrationFileManagerUploadFile",
                             "administration/file-manager/upload-file/{*targetUrl}",
                             new {Controller = "FileManager", Action = "UploadFile"});

            context.MapRoute("AdministrationFileManagerRename", "administration/file-manager/rename/{*targetUrl}",
                             new {Controller = "FileManager", Action = "Rename"});

            context.MapRoute("AdministrationFileManagerDelete", "administration/file-manager/delete/{*targetUrl}",
                             new {Controller = "FileManager", Action = "Delete"});

            context.MapRoute("AdministrationFileManagerList", "administration/file-manager/{*currentUrl}",
                             new {Controller = "FileManager", Action = "List"});

            context.MapRoute("AdministrationFilesCreateDirectory",
                             "administration/files/create-directory",
                             new {Controller = "Files", Action = "CreateDirectory"});

            context.MapRoute("AdministrationFilesUploadFile",
                             "administration/files/upload-file/",
                             new {Controller = "Files", Action = "UploadFile"});

            context.MapRoute("AdministrationFilesRename", "administration/files/{Id}/rename/",
                             new {Controller = "Files", Action = "Rename"},
                             new {Id = @"\d+"});

            context.MapRoute("AdministrationTemplates", "administration/templates/all/",
                             new { Controller = "Administration", Action = "Templates" });
        }
    }
}