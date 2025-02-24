Feature: Login Functionality
  As a user, I want to log in to the application to access restricted content.

  Scenario: Perform a successful login
    Given I am on the login page
    When I enter valid credentials
    Then I should see the secure area