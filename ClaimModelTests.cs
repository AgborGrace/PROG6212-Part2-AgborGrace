using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Tests
{
    /// <summary>
    /// Tests for Claim model validation and calculations
    /// </summary>
    public class ClaimModelTests
    {
        // Helper method to validate model
        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Claim_TotalAmount_CalculatesCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 100,
                HourlyRate = 450
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(45000, totalAmount);
        }

        [Fact]
        public void Claim_TotalAmount_WithDecimalValues_CalculatesCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 37.5m,
                HourlyRate = 425.50m
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(15956.25m, totalAmount);
        }

        [Fact]
        public void Claim_WithValidData_PassesValidation()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Dr. John Smith",
                LecturerEmail = "john.smith@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = 450,
                AdditionalNotes = "Teaching Programming 2B"
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Empty(validationResults); // No validation errors
        }

        [Fact]
        public void Claim_WithEmptyLecturerName_FailsValidation()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "", // Invalid
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 50,
                HourlyRate = 400
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("LecturerName"));
        }

        [Fact]
        public void Claim_WithInvalidEmail_FailsValidation()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "invalid-email", // Invalid format
                MonthYear = DateTime.Now,
                HoursWorked = 50,
                HourlyRate = 400
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("LecturerEmail"));
        }

        [Theory]
        [InlineData(0)] // Below minimum
        [InlineData(-10)] // Negative
        [InlineData(800)] // Above maximum
        public void Claim_WithInvalidHours_FailsValidation(decimal hours)
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = hours,
                HourlyRate = 400
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HoursWorked"));
        }

        [Theory]
        [InlineData(40)] // Below minimum
        [InlineData(0)] // Zero
        [InlineData(-100)] // Negative
        [InlineData(6000)] // Above maximum
        public void Claim_WithInvalidHourlyRate_FailsValidation(decimal rate)
        {
            // Arrange
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = rate
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HourlyRate"));
        }

        [Fact]
        public void Claim_WithTooLongNotes_FailsValidation()
        {
            // Arrange
            var longNotes = new string('A', 501); // 501 characters (max is 500)
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = 400,
                AdditionalNotes = longNotes
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("AdditionalNotes"));
        }

        [Fact]
        public void Claim_DefaultStatus_IsPending()
        {
            // Arrange & Act
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = 400
            };

            // Assert
            Assert.Equal(ClaimStatus.Pending, claim.Status);
        }

        [Fact]
        public void Claim_SubmittedAt_IsSetToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.Now.AddSeconds(-1);

            // Act
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = 400
            };

            var afterCreation = DateTime.Now.AddSeconds(1);

            // Assert
            Assert.True(claim.SubmittedAt >= beforeCreation);
            Assert.True(claim.SubmittedAt <= afterCreation);
        }

        [Fact]
        public void Claim_Documents_InitializedAsEmptyList()
        {
            // Arrange & Act
            var claim = new Claim
            {
                LecturerName = "Test Lecturer",
                LecturerEmail = "test@university.ac.za",
                MonthYear = DateTime.Now,
                HoursWorked = 100,
                HourlyRate = 400
            };

            // Assert
            Assert.NotNull(claim.Documents);
            Assert.Empty(claim.Documents);
        }
    }
}