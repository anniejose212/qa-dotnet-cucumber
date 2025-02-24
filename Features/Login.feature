Feature: Login Functionality
  As a user, I want to log in to the application to access restricted content.

  Scenario: Perform a successful login
    Given I am on the login page
    When I enter valid credentials
    Then I should see the secure area

  Scenario: Fail login with invalid username
    Given I am on the login page
    When I enter an invalid username and valid password
    Then I should see an error message

  Scenario: Fail login with invalid password
    Given I am on the login page
    When I enter a valid username and invalid password
    Then I should see an error message

  Scenario: Fail login with empty credentials
    Given I am on the login page
    When I enter empty credentials
    Then I should see an error message