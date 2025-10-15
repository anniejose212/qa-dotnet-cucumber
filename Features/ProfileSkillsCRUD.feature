Feature: Manage Skills on Profile
  As a user, I can manage the skills I have so others can see my technical expertise.

  Background:
    Given I am logged in as the default user
    And I open the Skills Tab



# ===== Positive Scenarios =====

@skills @positive @smoke
Scenario Outline: Add skill with level
  When I add skill "<Skill>" with level "<Level>"
  Then I should see a success message toast
  And I should see skill "<Skill>" with level "<Level>"

  Examples:
    | Skill | Level  |
    | C#    | Expert |

@skills @positive @smoke
Scenario Outline: Edit an existing skill
  When I add skill "<Skill>" with level "<Level>"
  When I update skill "<Skill>" to level "<NewLevel>"
  Then I should see a success message toast
  And I should see skill "<Skill>" with level "<NewLevel>"

  Examples:
    | Skill      | Level    | NewLevel     |
    | JavaScript | Beginner | Intermediate |

@skills @positive @smoke
Scenario Outline: Delete an existing skill
  When I add skill "<Skill>" with level "<Level>"
  When I delete skill "<Skill>" with level "<Level>"
  Then I should see a success message toast
  And I should not see skill "<Skill>" with level "<Level>"

  Examples:
    | Skill   | Level    |
    | Cypress | Beginner |

@skills @positive
@skills @positive
Scenario: Add multiple skills and verify total
  When I add the following skills
    | Skill    | Level        |
    | Cypress  | Beginner     |
    | Git      | Intermediate |
    | Docker   | Expert       |
    | Postman  | Expert       |
  Then I should see 4 skills in the list

  

# ===== Negative Scenarios =====

@skills @negative @regression
Scenario Outline: Prevent duplicate skill
  When I add skill "<ExistingSkill>" with level "<Level>"
  And I add skill "<DuplicateAttempt>" with level "<Level>"
  Then I should see an error message toast
  And I should see 1 occurrences of skill "<ExistingSkill>" with level "<Level>"

  Examples:
    | ExistingSkill | DuplicateAttempt | Level  |
    | Selenium      | Selenium         | Expert |


    @skills @negative @regression
Scenario Outline: Reject invalid level values
  When I attempt to add skill "<Skill>" with invalid level "<InvalidLevel>"
  Then I should see an error toast for invalid skill or level
  And I should not see skill "<Skill>" with level "<Level>"

  Examples:
    | Skill    | InvalidLevel |
    | Selenium | Savant       |

@skills @negative @currentbehaviour @knownissue
Scenario Outline: Prevent duplicate skill (case-insensitive and trimmed)
  When I add skill "<ExistingSkill>" with level "<Level>"
  And I add skill "<DuplicateAttempt>" with level "<Level>"
  Then I should see a success message toast
  And I should see 2 occurrences of skill "<ExistingSkill>" with level "<Level>"

  Examples:
    | ExistingSkill | DuplicateAttempt | Level  |
    | Jira          | jira             | Expert |

@skills @negative @desiredbehaviour @regression
Scenario Outline: Prevent duplicate skill (case-insensitive and trimmed) (desired)
  When I add skill "<ExistingSkill>" with level "<Level>"
  And I add skill "<DuplicateAttempt>" with level "<Level>"
  Then I should see an error message toast
  And I should see 1 occurrences of skill "<ExistingSkill>" with level "<Level>"

  Examples:
    | ExistingSkill | DuplicateAttempt | Level        |
    | SQL           | sql              | Intermediate |

@skills @negative @currentbehaviour @knownissue
Scenario Outline: Add non-existent skill
  When I add skill "<Skill>" with level "<Level>"
  Then I should see a success message toast
  And I should see skill "<Skill>" with level "<Level>"

  Examples:
    | Skill  | Level  |
    | Elvish | Expert |

@skills @negative @desiredbehaviour @regression
Scenario Outline: Add non-existent skill (desired)
  When I add skill "<Skill>" with level "<Level>"
  Then I should see an error message toast
  And I should not see skill "<Skill>" with level "<Level>"

  Examples:
    | Skill  | Level  |
    | Elvish | Expert |



# ===== Destructive and Security Scenarios =====

@skills @destructive @security @currentbehaviour @knownissue
Scenario Outline: Unsafe or destructive payload is still accepted (current behaviour)
  When I add potentially unsafe skill "<Skill>" with level "<Level>"
   Then check alert or row visibility for unsafe input

  Examples:
    | Skill                                                                  | Level        |
    | {DQ}DROP TABLE Skills;{DQ}                                             | Beginner     |
    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | Intermediate |
    | {DQ}{EQ:500}{DQ}                                                       | Expert       |
    

@skills @destructive @security @desiredbehaviour @regression
Scenario Outline: Unsafe or destructive payload should be rejected
  When I add potentially unsafe skill "<Skill>" with level "<Level>"
  Then the system should reject unsafe or malicious skill input
  Then I should see an error toast for invalid skill or level
  And I should not see skill "<Skill>" with level "<Level>"
  

  Examples:
    | Skill                                                                  | Level         |
    | {DQ}DROP TABLE Skills;{DQ}                                             | Beginner      |
    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | Intermediate  |
    | {DQ}{EQ:500}{DQ}                                                       | Expert        |


  @skills @destructive @security @currentbehaviour @regression
Scenario Outline: XSS payload accepted
  When I add potentially unsafe skill "<Skill>" with level "<Level>"
  Then check alert or row visibility for unsafe input

 

  Examples:
    | Skill                                     | Level           | AlertText         |
    | <img src=x onerror=alert('xss')>          | Beginner        | xss               |
    | <script>alert(1)</script>                 | Intermediate    | 1                 |
    | <script>alert(document.domain)</script>   | Beginner        | document.domain   |
    


@skills @destructive @security @desiredbehaviour @regression
Scenario Outline: XSS payload should be rejected(desired)
  When I add potentially unsafe skill "<Skill>" with level "<Level>"
  Then the system should reject unsafe or malicious skill input
  Then I should see an error toast for invalid skill or level
  And I should not see skill "<Skill>" with level "<Level>"
 

  Examples:
    | Skill                                     | Level           | AlertText         |
    | <img src=x onerror=alert('xss')>          | Beginner        | xss               |
    | <script>alert(1)</script>                 | Intermediate    | 1                 |
    | <script>alert(document.domain)</script>   | Beginner        | document.domain   |
