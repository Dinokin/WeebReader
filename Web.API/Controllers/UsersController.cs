using System;
using Microsoft.AspNetCore.Mvc;

namespace WeebReader.Web.API.Controllers
{
    public class UsersController : ApiController
    {
        public IActionResult Index(ushort page)
        {
            throw new NotImplementedException();
        }

        public IActionResult Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Add()
        {
            throw new NotImplementedException();
        }

        public IActionResult Edit(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Delete(Guid id)
        {
            throw new NotImplementedException();
        }
        
        public IActionResult ChangePassword(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult ChangeEmail(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult ResetPassword(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Lock(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Unlock(Guid id)
        {
            throw new NotImplementedException();
        }
        
        public IActionResult Self()
        {
            throw new NotImplementedException();
        }

        public IActionResult ChangePassword()
        {
            throw new NotImplementedException();
        }

        public IActionResult ChangeEmail()
        {
            throw new NotImplementedException();
        }

        public IActionResult ResetPassword(string token)
        {
            throw new NotImplementedException();
        }
    }
}