using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Aaina.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;

namespace Aaina.Web.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IChatService chatService;
        private readonly IGeneratePdf generatePdf;
        public ChatController(IChatService chatService, IGeneratePdf generatePdf)
        {
            this.chatService = chatService;
            this.generatePdf = generatePdf;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public async Task<IActionResult> Export(int id, int r, int t, string fr, string to, string rn, string pi)
        {
            DateTime fromDate = Convert.ToDateTime(fr);
            DateTime toDate = Convert.ToDateTime(to);
            var messageList = this.chatService.GetChatMessage(id, r, t,fromDate,toDate);
            ChatExportMessageDto response = new ChatExportMessageDto()
            {
                ChatList = messageList,
                FromDate = fromDate,
                ToDate = toDate,
                ProfileImage = pi,
                ReceiverName = rn
            };
            var result = await generatePdf.GetByteArray<ChatExportMessageDto>("/Views/chat/Export.cshtml", response);
            return this.File(result, "application/pdf", DateTime.Now.Ticks + ".pdf");
        }


    }
}