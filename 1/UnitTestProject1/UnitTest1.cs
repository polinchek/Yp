using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        // Тест 1: Успешная регистрация пользователя
        [TestMethod]
        public void RegisterUser_ValidData_ReturnsTrue()
        {
            // Arrange
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password123!";

            // Act
            bool result = _userService.RegisterUser(firstName, lastName, phone, email, password);

            // Assert
            Assert.IsTrue(result);
        }

        // Тест 2: Регистрация с пустым именем
        [TestMethod]
        public void RegisterUser_EmptyFirstName_ThrowsException()
        {
            // Arrange
            string firstName = "";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password123!";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.RegisterUser(firstName, lastName, phone, email, password));
        }

        // Тест 3: Регистрация с некорректным номером телефона
        [TestMethod]
        public void RegisterUser_InvalidPhone_ThrowsException()
        {
            // Arrange
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "12345"; // Некорректный номер телефона
            string email = "ivanov@example.com";
            string password = "Password123!";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.RegisterUser(firstName, lastName, phone, email, password));
        }

        // Тест 4: Регистрация с паролем, содержащим только буквы
        [TestMethod]
        public void RegisterUser_PasswordWithoutDigits_ThrowsException()
        {
            // Arrange
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password"; // Пароль без цифр

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.RegisterUser(firstName, lastName, phone, email, password));
        }

        // Тест 5: Успешная авторизация пользователя
        [TestMethod]
        public void LoginUser_ValidData_ReturnsTrue()
        {
            // Arrange
            string email = "ivanov@example.com";
            string password = "Password123!";

            // Act
            bool result = _userService.LoginUser(email, password);

            // Assert
            Assert.IsTrue(result);
        }

        // Тест 6: Авторизация с пустым email
        [TestMethod]
        public void LoginUser_EmptyEmail_ThrowsException()
        {
            // Arrange
            string email = "";
            string password = "Password123!";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.LoginUser(email, password));
        }

        // Тест 7: Авторизация с некорректным паролем
        [TestMethod]
        public void LoginUser_InvalidPassword_ThrowsException()
        {
            // Arrange
            string email = "ivanov@example.com";
            string password = "123"; 

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.LoginUser(email, password));
        }

        // Тест 8: Авторизация с пустым паролем
        [TestMethod]
        public void LoginUser_EmptyPassword_ThrowsException()
        {
            // Arrange
            string email = "ivanov@example.com";
            string password = "";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.LoginUser(email, password));
        }

        // Тест 9: Регистрация с уже зарегистрированным email
        [TestMethod]
        public void RegisterUser_DuplicateEmail_ThrowsException()
        {
            // Arrange
            string firstName = "Иван";
            string lastName = "Иванов";
            string phone = "+79991234567";
            string email = "ivanov@example.com";
            string password = "Password123!";

            // Act
            _userService.RegisterUser(firstName, lastName, phone, email, password); // Регистрация первого пользователя

            // Assert
            Assert.ThrowsException<ArgumentException>(() => _userService.RegisterUser(firstName, lastName, phone, email, password)); // Попытка регистрации с тем же email
        }

        // Тест 10: Авторизация с учетом регистра в email
        [TestMethod]
        public void LoginUser_CaseInsensitiveEmail_ReturnsTrue()
        {
            // Arrange
            string email = "Ivanov@example.com"; // Email с заглавной буквой
            string password = "Password123!";

            // Act
            bool result = _userService.LoginUser(email, password);

            // Assert
            Assert.IsTrue(result);
        }
    }
}