using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

namespace Main_Lecture_4_Xml_
{
    class Program
    {

        static void Main(string[] args)
        {
            List<Student> ImportStudents = new List<Student>();
           
            // Import
            Import(ImportStudents);
            Console.WriteLine("V.1");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nList after Import Students. Count - {ImportStudents.Count}");
            Console.ResetColor();
            foreach (var st in ImportStudents)
            {
                Console.WriteLine(st.ToString());
            }

            // V.2  I just want to try this method but I do not know how to add ExtraDataElement and Course
            XDocument document2 = XDocument.Load("Input_students.xml");
            var studentsElement2 = from xe in document2.Element("Students").Elements("Student")   //child
                                   select new Student
                                   {
                                       FirstName = xe.Attribute("firstName").Value,
                                       LastName = xe.Attribute("lastName").Value,
                                       Birthday = DateTime.ParseExact(xe.Element("BirthDate").Value, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                                       Email = xe.Element("Email").Value,
                                       Phone = xe.Element("PhoneNumber").Value,
                                       GitHubLink = xe.Element("GitHubLink").Value

                                   };
            Console.WriteLine("\nV.2");

            foreach (var item in studentsElement2)
            {
                Console.WriteLine($"{item.FirstName} - {item.LastName} - {item.GitHubLink}");
            }

            //Export
            var result = Export(ImportStudents);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nFile after Export Students. Count - {ImportStudents.Count}");
            Console.ResetColor();
            Console.WriteLine($"\n{result}");

            //Add new student
            Student newStudent = new Student()
            {
                FirstName = "Ketrin",
                LastName = "Shynkarenko",
                Birthday = new DateTime(1987,12,05),
                Email = "katja.shynkarenko@gmail.com",
                Phone = "0936233319",
                GitHubLink = "katya-shynkarenko"
            };
            ImportStudents.Add(newStudent);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nList after added new Student. Count - {ImportStudents.Count}");
            Console.ResetColor();
            foreach (var st in ImportStudents)
            {
                Console.WriteLine(st.ToString());
            }

            var result2 = Export(ImportStudents);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nFile after added new Student. Count - {ImportStudents.Count}");
            Console.ResetColor();
            Console.WriteLine($"{result2}");

            Import(ImportStudents);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\nList after second Import Students. Count - {ImportStudents.Count}");
            Console.ResetColor();
            foreach (var st in ImportStudents)
            {
                Console.WriteLine(st.ToString());
            }
            Console.ReadKey();
        }
        
        private static void Import(List<Student> ImportStudents)
        {
            ImportStudents?.Clear();
            
            XDocument documentImp = new XDocument();
            documentImp = XDocument.Load("Input_students.xml");
            var studentsElement = documentImp.Elements().First();           // root
            
            foreach (var student in studentsElement.Elements())          //child
            {
                Student importStudent = new Student();
                importStudent.FirstName = student.Attribute("firstName").Value;
                importStudent.LastName = student.Attribute("lastName").Value;
                importStudent.Birthday = ImportStudentBirthday(student);
                importStudent.Phone = student.Element("PhoneNumber").Value;
                importStudent.Email = student.Element("Email").Value;
                importStudent.GitHubLink = student.Element("GitHubLink").Value;
                ImportExtraData(importStudent, student.Elements().SelectMany(p => p.Elements("ExtraDataElement")));
                ImportCourses(importStudent, student.Elements().SelectMany(p => p.Elements("Course")));
                ImportStudents.Add(importStudent);
            }

        }
        private static DateTime ImportStudentBirthday(XElement student)
        {
            DateTime birthDate = DateTime.ParseExact(student.Element("BirthDate").Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            return birthDate;
        }

        private static void ImportExtraData(Student importStudent, IEnumerable<XElement> extraDataElements)
        {
            foreach (XElement extraDataElement in extraDataElements)
            {
                importStudent.ExtraData[extraDataElement.Attribute("name").Value] = extraDataElement.Value;
            }
        }

        private static void ImportCourses(Student importedStudent, IEnumerable<XElement> courseElements)
        {
            foreach (XElement courseElement in courseElements)
            {
                importedStudent.Courses.Add(courseElement.Value);
            }
        }

        public static XDocument Export(List<Student> ImportStudents)
        {
            XDocument documentExp = new XDocument();
            XElement root = new XElement("Students");
            documentExp.Add(root);

            foreach (var student in ImportStudents)
            {
                XElement studentElement = new XElement("Student", 
                    new XAttribute("firstName", student.FirstName), 
                    new XAttribute("lastName", student.LastName));

                root.Add(studentElement);
                studentElement.Add(new XElement("BirthDate", GetBirthDate(student.Birthday)));
                studentElement.Add(new XElement("PhoneNumber", student.Phone));
                studentElement.Add(new XElement("Email", student.Email));
                studentElement.Add(new XElement("GitHubLink", student.Email));

                XElement extraDataElement = new XElement("ExtraData");
                studentElement.Add(extraDataElement);
                foreach (var extraData in student.ExtraData)
                {
                    extraDataElement.Add(new XElement("ExtraDataElement", new XAttribute("name", extraData.Key), extraData.Value));
                }
                XElement coursesElement = new XElement("Courses");
                studentElement.Add(coursesElement);
                foreach (var course in student.Courses)
                {
                    coursesElement.Add(new XElement("Course", course));
                }           
            }

            documentExp.Save("Input_students.xml");
            return documentExp;      
        }

        private static string GetBirthDate(DateTime studentBirthday)
        {
            return studentBirthday.ToString("dd.MM.yyy", CultureInfo.InvariantCulture);
        }

    }
}



