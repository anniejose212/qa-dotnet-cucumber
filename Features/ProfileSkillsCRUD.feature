Feature: Manage Skills on Profile
  As a user I would be able to show what  skills I know.
So that the people seeking for skills can look at what details I hold.

  Background:
    Given I am on the login page
    When I enter valid credentials
    Then I should see the secure area
    And I open the Skills Tab

  Scenario: Add, display, update, and delete a skill
    When I add a skill "Automation Testing" with level "Expert"
    Then I should see a success toast for skills
    And the skill "Automation Testing" with level "Expert" should exist

    And print the skill

    When I change the skill level to "Intermediate"
    Then I should see a success toast for skills
    And the skill "Automation Testing" with level "Intermediate" should exist
    And print the skill

    When I delete the skill
    Then I should see a success toast for skills
    And the skill "Automation Testing" should not appear in my profile
