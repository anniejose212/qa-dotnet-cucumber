Feature: Manage Languages on Profile
  As a user, I can manage the languages I know so others can see my details.

   Background:
    Given I am logged in as the default user

# ===== Positive Scenarios =====

@languages @positive @smoke
Scenario Outline: Add language with level
  When I add language "<Language>" with level "<Level>"
  Then I should see a success toast
  And I should see language "<Language>" with level "<Level>"

  Examples:
    | Language | Level          |
    | English  | Fluent         |

@languages @positive @smoke
Scenario Outline: Edit an existing language
  When I add language "<Language>" with level "<Level>"
  When I update language "<Language>" to level "<NewLevel>"
  Then I should see a success toast
  And I should see language "<Language>" with level "<NewLevel>"

  Examples:
    | Language            | Level          | NewLevel       |
    | Traditional Chinese | Conversational | Basic          |
    

@languages @positive @smoke
Scenario Outline: Delete an existing language
  When I add language "<Language>" with level "<Level>"
  When I delete language "<Language>" with level "<Level>"
  Then I should see a success toast
  And I should not see language "<Language>" with level "<Level>"

  Examples:
    | Language | Level  |
    | German   | Basic  |
  
@languages @positive
Scenario Outline: View all languages after adding multiple
  When I add the following languages
  | Language | Level  |
    | Japanese | Basic  |
    | Chinese  | Fluent |
  Then I should see a success toast
  Then I should see 2 languages in the list


# ===== Negative Scenarios =====

@languages @negative
Scenario Outline: Add New button should be hidden after adding 4 languages
  When I add the following languages

    | Language | Level        |
    | English  | Basic    |
    | French   | Conversational |
    | German   | Native/Bilingual       |
    | Spanish  | Fluent      |
  Then the Add New button should be hidden for languages

  @languages @negative @regression
Scenario Outline: Reject invalid level values
  When I attempt to add language "<Language>" with invalid level "<InvalidLevel>"
  Then I should see an error toast for invalid language or level
  And I should not see language "<Language>" with level "<Level>"

  Examples:
    | Language | InvalidLevel |
    | Hindi    | Virtuoso    |

@languages @negative @regression
Scenario Outline: Prevent duplicate language
  When I add language "<ExistingLanguage>" with level "<Level>"
  And I add language "<DuplicateAttempt>" with level "<Level>"
  Then I should see an error toast
  And I should see 1 occurrences of language "<ExistingLanguage>" with level "<Level>"

  Examples:
    | ExistingLanguage | DuplicateAttempt | Level  |
    | Russian          | Russian          | Fluent |

@languages @negative @currentbehaviour @knownissue
Scenario Outline: Prevent duplicate language (case-insensitive and trimmed)
  When I add language "<ExistingLanguage>" with level "<Level>"
  And I add language "<DuplicateAttempt>" with level "<Level>"
  Then I should see a success toast
  And I should see 2 occurrences of language "<ExistingLanguage>" with level "<Level>"

  Examples:
    | ExistingLanguage | DuplicateAttempt | Level  |
    | Portuguese       | portuguese       | Fluent |

@languages @negative @desiredbehaviour @regression
Scenario Outline: Prevent duplicate language (case-insensitive and trimmed) (desired)
  When I add language "<ExistingLanguage>" with level "<Level>"
  And I add language "<DuplicateAttempt>" with level "<Level>"
  Then I should see an error toast
  And I should see 1 occurrences of language "<ExistingLanguage>" with level "<Level>"

  Examples:
    | ExistingLanguage | DuplicateAttempt | Level  |
    | Russian          | russian          | Fluent |

@languages @negative @currentbehaviour @knownissue
Scenario Outline: Add non-existent language
 When I add language "<Language>" with level "<Level>"
  Then I should see a success toast
  And I should see language "<Language>" with level "<Level>"


  Examples:
    | Language | Level  |
    | Elvish   | Fluent |

@languages @negative @desiredbehaviour @regression
Scenario Outline: Add non-existent language (desired)
  When I add language "<Language>" with level "<Level>"
  Then I should see an error toast
   And I should not see language "<Language>" with level "<Level>"

  Examples:
    | Language | Level  |
    | Elvish   | Fluent |


# ===== Destructive and Security Scenarios =====

@languages @destructive @security @currentbehaviour @knownissue
Scenario Outline: Unsafe or destructive payload is still accepted (current behaviour)
  When I add potentially unsafe language "<Language>" with level "<Level>"
  Then check alert or row visibility for unsafe language input

  Examples:
    | Language                                                               | Level  |
    | {DQ}DROP TABLE Languages;{DQ}                                          | Basic  |
    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | Basic  |
    | {DQ}{EQ:500}{DQ}                                                       | Fluent |
    

    @languages @destructive @security @desiredbehaviour @regression
Scenario Outline: Unsafe or destructive payload should be rejected
  When I add potentially unsafe language "<Language>" with level "<Level>"
  Then the system should reject unsafe input
  Then I should see an error toast for invalid language or level
  And I should not see language "<Language>" with level "<Level>"
  

  Examples:
    | Language                                                               | Level  |
    | {DQ}DROP TABLE Languages;{DQ}                                          | Basic  |
    | AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA | Basic  |
    | {DQ}{EQ:500}{DQ}                                                       | Fluent |
    


   @languages @destructive @security @currentbehaviour @regression
Scenario Outline: XSS payload accepted
  When I add potentially unsafe language "<Language>" with level "<Level>"
  Then check alert or row visibility for unsafe language input


  
    Examples:
      | Language                                | Level  | AlertText       |
      | <img src=x onerror=alert('xss')>        | Basic  | xss             |
      | <script>alert(1)</script>               | Fluent | 1               |
      | <script>alert(document.domain)</script> | Basic  | document.domain | 
     


     @languages @destructive @security @desiredbehaviour @regression
Scenario Outline: XSS payload should be rejected(desired)
  When I add potentially unsafe language "<Language>" with level "<Level>"
  Then the system should reject unsafe input
  Then I should see an error toast for invalid language or level
  And I should not see language "<Language>" with level "<Level>"
 
   Examples:
      | Language                                | Level  |
      | <img src=x onerror=alert('xss')>        | Basic  |
      | <script>alert(1)</script>               | Fluent |
      | <script>alert(document.domain)</script> | Basic  | 
      







 

  
