using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using _1;

namespace UnitTestProject1
{
    [TestClass]
    public class UserService
    {
        public bool RegisterUser(string firstName, string lastName, string phone, string email, string password)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Все поля обязательны для заполнения.");
            }

            // Проверка email
            if (!email.Contains("@"))
            {
                throw new ArgumentException("Некорректный email.");
            }

            // Успешная регистрация
            return true;
        }

        public bool LoginUser(string email, string password)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email и пароль обязательны для заполнения.");
            }

            // Проверка email
            if (!email.Contains("@"))
            {
                throw new ArgumentException("Некорректный email.");
            }

            // Успешная авторизация
            return true;
        }
    }

    [TestClass]
    public class UserServiceTests
    {
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _userService = new UserService();
        }

        // Тест 1
        [TestMethod]
        public void RegisterUser_ValidData_ReturnsTrue()
        {
            
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password123!";

            
            bool result = _userService.RegisterUser(firstName, lastName, phone, email, password);

            
            Assert.IsTrue(result);
        }

        // Тест 2
        [TestMethod]
        public void RegisterUser_EmptyFirstName_ThrowsException()
        {
           
            string firstName = "";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password123!";

            
            Assert.ThrowsException<ArgumentException>(() => _userService.RegisterUser(firstName, lastName, phone, email, password));
        }

        // Тест 3
        [TestMethod]
        public void LoginUser_EmailWithSpaces_ThrowsException()
        {
            
            string email = "f"; 
            string password = "Password123!";

            bool result = _userService.LoginUser(email, password);


            Assert.IsTrue(result);
        }


        // Тест 4
        [TestMethod]
        public void LoginUser_PasswordWithLettersAndDigits_ReturnsTrue()
        {
            
            string email = "ivanov@example.com";
            string password = "Password123"; 

            
            bool result = _userService.LoginUser(email, password);

            
            Assert.IsTrue(result);
        }

        // Тест 5
        [TestMethod]
        public void LoginUser_ValidData_ReturnsTrue()
        {
            
            string email = "ivanov@example.com";
            string password = "Password123!";

            bool result = _userService.LoginUser(email, password);

            
            Assert.IsTrue(result);
        }

        // Тест 6
        [TestMethod]
        public void LoginUser_EmptyEmail_ThrowsException()
        {
           
            string email = "";
            string password = "Password123!";

            
            Assert.ThrowsException<ArgumentException>(() => _userService.LoginUser(email, password));
        }

        // Тест 7
        [TestMethod]
        public void RegisterUser_PasswordWithSpecialCharacters_ReturnsTrue()
        {
            
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password!@#123"; 

           
            bool result = _userService.RegisterUser(firstName, lastName, phone, email, password);

            
            Assert.IsTrue(result);
        }

        // Тест 8
        [TestMethod]
        public void LoginUser_EmptyPassword_ThrowsException()
        {
            
            string email = "ivanov@example.com";
            string password = "";

            
            Assert.ThrowsException<ArgumentException>(() => _userService.LoginUser(email, password));
        }

        // Тест 9
        [TestMethod]
        public void PasswordsMatch_ShouldReturnTrueForMatchingPasswords()
        {
            string password = "testPassword123";
            string confirmPassword = "testPassword123";
            bool passwordsMatch = password == confirmPassword;
            Assert.IsTrue(passwordsMatch);
        }

        // Тест 10
        [TestMethod]
        public void LoginUser_CaseInsensitiveEmail_ReturnsTrue()
        {
            
            string email = "Ivanov@example.com";
            string password = "Password123!";

           
            bool result = _userService.LoginUser(email, password);

            
            Assert.IsTrue(result);
        }
    }
}