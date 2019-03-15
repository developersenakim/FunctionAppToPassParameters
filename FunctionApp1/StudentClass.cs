using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1
{
    class StudentData
    {
        public string Department;
        public string Name;
        public string Email;
        public string Student_group;
        public string Manager_yn;
        public string Enabled_user_yn;

        public StudentData(string department, string name, string email, string studentgroup, string manageryn, string enableuseryn)
        {
              Department = department;
            Name = name;
            Email = email;
            Student_group = studentgroup;
            Manager_yn = manageryn;
            Enabled_user_yn = enableuseryn;
        }
    }
}