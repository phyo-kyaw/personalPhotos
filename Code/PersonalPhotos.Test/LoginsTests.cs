using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PersonalPhotos.Test
{
    public class LoginsTests
    {
        private readonly LoginsController _loginsController;
        private readonly Mock<ILogins> _logins;
        private readonly Mock<IHttpContextAccessor> _accessor;
        public LoginsTests()
        {
            _logins = new Mock<ILogins>();

            var session = Mock.Of<ISession>();
            var httpContext = Mock.Of<HttpContext>(x => x.Session == session);

            _accessor = new Mock<IHttpContextAccessor>();
            _accessor.Setup(x => x.HttpContext).Returns(httpContext);

            _loginsController = new LoginsController(_logins.Object, _accessor.Object);
        }

        [Fact]
        public void Index_GivenNotReturnUrl_ReturnLoginView()
        {
            var result = _loginsController.Index();

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);

            var result1 = ( _loginsController.Index()  as ViewResult);
            Assert.NotNull(result1);
            Assert.Equal("login", result1.ViewName, ignoreCase: true);

        }

        [Fact]
        public async Task Login_GivenModelStageInvalid_ReturnLoginView()
        {
            _loginsController.ModelState.AddModelError("Test", "Test");

            var result = await _loginsController.Login(Mock.Of<LoginViewModel>()) as ViewResult;
            Assert.Equal("Login", result.ViewName, ignoreCase: true);
        }

        [Fact]
        public async Task Login_GivenCorrectPassword_RedirectToDisplayAction()
        {
            const string password = "123";
            var modelView = Mock.Of<LoginViewModel>(x => x.Email == "a@b.com" && x.Password == password);
            var model = Mock.Of<User>(x => x.Password == password);

            _logins.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(model);
            var result = await _loginsController.Login(modelView);

            Assert.IsType<RedirectToActionResult>(result);
            //Assert.Equal("display", result.ToString(), ignoreCase: true);

        }

    }
}
