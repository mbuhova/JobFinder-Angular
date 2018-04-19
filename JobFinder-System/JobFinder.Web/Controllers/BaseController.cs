namespace JobFinder.Web.Controllers
{
    using System.Web.Mvc;
    using JobFinder.Data;

    public class BaseController : Controller
    {
        public BaseController(IJobFinderData data)
        {
            this.Data = data;                     
        }

        protected IJobFinderData Data { get; private set; }

        public ActionResult Error()
        {
            return this.View();
        }
    }
}