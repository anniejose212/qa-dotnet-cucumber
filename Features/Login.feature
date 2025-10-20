Feature: Login Functionality
  As a user, I want to log in to the application to access restricted content.


  Background:
	Given I am on the login modal
   
@positive @smoke
  Scenario Outline: Perform a successful login
    Given I am on the login modal
    When I enter valid credentials
    Then I should see the secure area


     # ===== Negative Scenarios =====
     @negative
 Scenario Outline: Fail login with invalid username
    When I enter an invalid username and valid password
    Then I should see an error message 
    
    
   @negative
  Scenario Outline: Fail login with invalid password
    When I enter a valid username and invalid password
	Then I should see an error message 
    @negative
  Scenario Outline: Fail login with empty credentials
    When I enter empty credentials
	Then I should see an error message 

    @negative @currentbehaviour
Scenario Outline: Lockout after repeated failed login attempts
  When I enter an valid username and invalid password multiple times
  Then I should see an error message

  Examples:
    | Username                           | Password |
    | annie.jose1202@gmail.com           | 12345   |
   
    

@negative @desiredbehaviour
Scenario Outline:Account should be temporarily locked after too many failed logins
  When I enter an valid username and invalid password multiple times
  Then I should see an error message
  And I should not be able to login again for a cooldown period

  Examples:
    | Username                         | Password        |
    | annie.jose1202@gmail.com         | 12345          |
    
    @negative 
 Scenario Outline: Login with whitespace around credentials
  When I enter valid credentials with leading or trailing spaces
  Then I should see an error message

  Examples:
    | Username                      | Password       |
    | "  annie.jose1202@gmail.com " | " 123456 "     |


