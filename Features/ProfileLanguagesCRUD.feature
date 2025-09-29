
Feature: Manage Languages on Profile
 As a user I would be able to show what languages  I know.
So that the people seeking for languages can look at what details I hold.

  Background:
  Given I am on the login page
  When I enter valid credentials
  Then I should see the secure area

  Scenario: Add, update, display and delete a language
    When I add a language "Spanish" with level "Fluent"
    Then I should see a success toast
    And the language "Spanish" with level "Fluent" should exist
    And print the language

    When I change the language level to "Conversational"
    Then I should see a success toast
    And the language "Spanish" with level "Conversational" should exist
    And print the language

    When I delete the language
    Then I should see a success toast
    And the language "Spanish" should not appear in my profile
    
