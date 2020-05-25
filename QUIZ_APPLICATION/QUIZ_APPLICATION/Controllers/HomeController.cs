using QUIZ_APPLICATION.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QUIZ_APPLICATION.Controllers
{
    public class HomeController : Controller
    {
        OnlineExameEntities onlineExamDB = new OnlineExameEntities();

        public ActionResult Index()
        {
            //This Condition is used to when login Admin the Admin can not to redirect back page 
            if (Session["AD_ID"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        public ActionResult Dashboard()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        //This Event is used to when Logout Admin then Admin can to redirect back page 
        [HttpGet]
        public ActionResult TeacherLogout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }  
        
        [HttpGet]
        public ActionResult TeacherLogin()
        {
            return View();
        }

        //This Event is Used to Admin Login .
        //Only Athourised Person is Access
        [HttpPost]
        public ActionResult TeacherLogin(TBL_ADMIN admin)
        {
            TBL_ADMIN tblAdmin = onlineExamDB.TBL_ADMIN.Where(x => x.AD_NAME == admin.AD_NAME && x.AD_PASSWORD == admin.AD_PASSWORD).SingleOrDefault();
            if (tblAdmin != null)
            {
                Session["AD_ID"] = tblAdmin.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid Username or Password";
            }
            return View();
        }

        // This Function are Used to Retrive all Data Database to On Page 
        //This Data is Display Admin Dependant Which Admin is Add The Add te Subject only data have watch that admin
        // Other Subject Can not Watch other Admin
        //Only Authorized Admin can Watch its Own Register Subject
        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }
            int adid = Convert.ToInt16(Session["AD_ID"]);
            List<TBL_CATEGORY> category = onlineExamDB.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == adid).OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = category;
            return View();
        }

        //This functions is used for 
        // When Admin is Login the Admin is Register the Subject 
        [HttpPost]
        public ActionResult AddCategory(TBL_CATEGORY cat)
        {

            Random r = new Random();
            List<TBL_CATEGORY> category = onlineExamDB.TBL_CATEGORY.OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] = category;

            TBL_CATEGORY tbl = new TBL_CATEGORY();
            tbl.CAT_NAME = cat.CAT_NAME;
            tbl.Cat_Encyptedstring = EncryptCategoryID.Encrypt(cat.CAT_NAME.Trim() + r.Next().ToString(), true);
            tbl.CAT_PK_ADID = Convert.ToInt32(Session["AD_ID"].ToString());
            onlineExamDB.TBL_CATEGORY.Add(tbl);
            onlineExamDB.SaveChanges();
            return RedirectToAction("AddCategory");
        }
        [HttpGet]
        public ActionResult AddQuestions()
        {
            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> cat = onlineExamDB.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == sid).ToList();
            ViewBag.list = new SelectList(cat, "CAT_ID", "CAT_NAME");
            return View();
        }
        [HttpPost]
        public ActionResult AddQuestions(TBL_QUESTION question)
        {
            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> cat = onlineExamDB.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == sid).ToList();
            ViewBag.list = new SelectList(cat, "CAT_ID", "CAT_NAME");

            TBL_QUESTION quest = new TBL_QUESTION();
            quest.Q_TEXT = question.Q_TEXT;
            quest.OPA = question.OPA;
            quest.OPB = question.OPB;
            quest.OPC = question.OPC;
            quest.OPD = question.OPD;
            quest.COP = question.COP;
            quest.Q_FK_CATID = question.Q_FK_CATID;
            onlineExamDB.TBL_QUESTION.Add(quest);
            onlineExamDB.SaveChanges();
            TempData["msg"] = "Question added Successfully.....!";
            TempData.Keep();
            return RedirectToAction("AddQuestions");

        }
        public ActionResult ViewAllQuestion(int? id)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("TeacherLogin");
            }

            if (id == 0)
            {
                return RedirectToAction("Dashboard");
            }
            return View(onlineExamDB.TBL_QUESTION.Where(x => x.Q_FK_CATID == id).ToList());
        }

        [HttpGet]
        public ActionResult StudentLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StudentLogin(TBL_STUDENT std)
        {
            TBL_STUDENT student = onlineExamDB.TBL_STUDENT.Where(x => x.STD_NAME == std.STD_NAME && x.STD_PASSWORD == std.STD_PASSWORD).SingleOrDefault();
            if (student == null)
            {
                ViewBag.LoginErrorMsg = "Invalid Username && Password";
            }
            else
            {
                Session["STD_ID"] = std.STD_ID;
                
                return RedirectToAction("StudentExam");
            }
            return View();
        }

        [HttpGet]
        public ActionResult StudentRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StudentRegister(TBL_STUDENT std, HttpPostedFileBase imgFile)
        {
            TBL_STUDENT student = new TBL_STUDENT();
            try
            {
                student.STD_NAME = std.STD_NAME;
                student.STD_PASSWORD = std.STD_PASSWORD;
                student.STD_Image = UploadImage(imgFile);
                onlineExamDB.TBL_STUDENT.Add(student);
                onlineExamDB.SaveChanges();
                return RedirectToAction("StudentLogin");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Data Could Not be Inserted.......!";
            }

            return View();
        }
        public string UploadImage(HttpPostedFileBase imgFile)
        {
            string path = "-1";
            try
            {
                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(imgFile.FileName);

                    if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                    {
                        Random random = new Random();
                        path = Path.Combine(Server.MapPath("~/Content/UloadedImage"), random + Path.GetFileName(imgFile.FileName));
                        imgFile.SaveAs(path);
                        path = "~/Content/UloadedImage/" + random + Path.GetFileName(imgFile.FileName);
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            return path;
        }

        public ActionResult StudentExam()
        {
            if (Session["STD_ID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult StudentExam(String room)
        {
            TempData["Score"] = 0;
            List<TBL_CATEGORY> category = onlineExamDB.TBL_CATEGORY.ToList();
            foreach (var item in category)
            {
                if (item.Cat_Encyptedstring == room)
                {
                    List<TBL_QUESTION> question = onlineExamDB.TBL_QUESTION.Where(x => x.Q_FK_CATID == item.CAT_ID).ToList();
                    Queue<TBL_QUESTION> queue = new Queue<TBL_QUESTION>();
                    foreach (TBL_QUESTION addquestion in question)
                    {
                        queue.Enqueue(addquestion);
                    }
                    TempData["Question"] = queue;
                    TempData["Score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStartPage");
                }
                else
                {
                    ViewBag.Error = "No Room Found.......!";
                }
            }
            return View();
        }

        public ActionResult QuizStartPage()
        {

            if (Session["STD_ID"] == null)
            {
                return RedirectToAction("StudentLogin");
            }

            TBL_QUESTION questions = null;
            if (TempData["Question"] != null)
            {
                Queue<TBL_QUESTION> qlist = (Queue<TBL_QUESTION>)TempData["Question"];
                if (qlist.Count > 0)
                {
                    questions = qlist.Peek();
                    qlist.Dequeue();
                    TempData["Question"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }

            return View(questions);
        }
        [HttpPost]
        public ActionResult QuizStartPage(TBL_QUESTION question)
        {
            string correctAns = null;

            if (question.OPA != null)
            {
                correctAns = "A";
            }
            else if (question.OPB != null)
            {
                correctAns = "B";
            }
            else if (question.OPC != null)
            {
                correctAns = "C";
            }
            else if (question.OPD != null)
            {
                correctAns = "D";
            }

            if (correctAns.Equals(question.COP))
            {
                TempData["Score"] = Convert.ToInt32(TempData["Score"]) + 1;
            }
            TempData.Keep();
            return RedirectToAction("QuizStartPage");
        }
        public ActionResult EndExam()
        {
            return View();
        }
        
    }
}