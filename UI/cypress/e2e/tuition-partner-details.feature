Feature: User can view full details of a Tuition Parner
  Scenario: page title is 'Name of Tuition Partner'
    Given a user has arrived on the 'Tuition Partner' page for 'Toranj Tuition'
    Then the page's title is 'Toranj Tuition'

  Scenario: user clicks service name
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: user directly accesses details page using the full name
    Given a user has arrived on the 'Tuition Partner' page for 'Action Tutoring'
    Then the page URL ends with '/action-tutoring'
    And the heading should say 'Action Tutoring'

  Scenario: user directly accesses details page using the SEO name
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the page URL ends with '/bright-heart-education'
    And the heading should say 'Bright Heart Education'

  Scenario: don’t show email address where TP has not provided information
    Given a user has arrived on the 'Tuition Partner' page for 'pearson'
    Then TP has not provided the information in the 'Email address' section

  Scenario: don’t show phone number where TP has not provided information
    Given a user has arrived on the 'Tuition Partner' page for 'pearson'
    Then TP has not provided the information in the 'Phone Number' section

  Scenario: show Contact Details where TP has provided information
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then TP has provided full contact details

  Scenario: home page is selected 
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    When they click 'Home'
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: tuition partner details page linked from search results page has 'Back to search results' back link
    Given a user has arrived on the 'Tuition Partner' page for 'Bright Heart Education' after searching for 'Key stage 1 English'
    Then the back link's text is 'Back to search results'

  Scenario: back is selected return to search results page
    Given a user has arrived on the 'Tuition Partner' page for 'Bright Heart Education' after searching for 'Key stage 1 English'
    When they click 'Back'
    Then the page URL ends with '/search-results'
    And the search details include 'Key stage 1 English'

  Scenario: back is selected return to search results page with multiple subjects
    Given a user has arrived on the 'Tuition Partner' page for 'Bright Heart Education' after searching for 'Key stage 1 English, Key stage 2 Maths'
    When they click 'Back'
    Then the page URL ends with '/search-results'
    And the search details include 'Key stage 1 English, Key stage 2 Maths'

  Scenario: quality assured tuition partner and payment details is initially hidden
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the quality assured tuition partner details are hidden
    And the payment details are hidden

  Scenario: user clicks quality assured tuition partner details summary
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    When they click 'What is a quality assured tuition partner?'
    Then the quality assured tuition partner details are shown

  Scenario: tuition partner website link is displayed
    Given a user has arrived on the 'Tuition Partner' page for 'tlc-live'
    Then the tuition partner's website link is displayed
    And the tuition partners website link starts with 'http://'

  Scenario: user has access to funding link page
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    When they click funding and reporting link
    Then they will see the funding reporting header
    And  they will click the back link
    Then they redirects to the tuition partners website link with bright-heart-education
  
  Scenario: locations covered table is not displayed as default
    Given a user has arrived on the 'Tuition Partner' page for 'tutor-doctor-(cambridge-tuition-limited)'
    Then the tuition partner locations covered table is not displayed

  Scenario: locations covered table is displayed when show-locations-covered=true
    Given a user has arrived on the 'Tuition Partner' page for 'tutor-doctor-(cambridge-tuition-limited)'
    When they set the 'show-locations-covered' query string parameter value to 'true'
    Then the tuition partner locations covered table is displayed

  Scenario: tuition cost table shows available tuition types
    Given a user has arrived on the 'Tuition Partner' page for '<tution-partner>'
    Then the tuition partner pricing table is displayed for '<tuition-types>'
    Examples:
    | tution-partner | tuition-types |
    | Fledge Tuition Ltd | Online |
    | FFT Education | In school |
    | career-tree | In school, Online |

  Scenario: tuition cost blurb states pricing uniformity
    Given a user has arrived on the 'Tuition Partner' page for 'Fledge Tuition Ltd'
    Then the tuition cost information states declares no differences

  Scenario: tuition cost blurb states pricing differences
    Given a user has arrived on the 'Tuition Partner' page for 'Reed Tutors'
    Then the tuition cost information states declares differences

  Scenario: full pricing tables are not displayed as default
    Given a user has arrived on the 'Tuition Partner' page for 'Fledge Tuition Ltd'
    Then the tuition partner full pricing tables are not displayed
    And the tuition partner pricing table is displayed

  Scenario: full pricing tables are displayed when show-full-pricing=true
    Given a user has arrived on the 'Tuition Partner' page for 'Fledge Tuition Ltd'
    When they set the 'show-full-pricing' query string parameter value to 'true'
    Then the tuition partner full pricing tables are displayed
    And the tuition partner pricing table is not displayed

  Scenario: subjects covered by a tuition partner are in alphabetical order in the 'search results' page
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the subjects covered by a tuition partner are in alphabetical order 

  Scenario: Tuition partner details are displayed correctly when arriving on the results page
     Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
     Then all tuition partner details are populated correctly

Scenario: Logos are displayed for tution partners
    Given a user has arrived on the 'Tuition Partner' page for 'nudge-education'
    Then the logo is shown

Scenario: Logos are not displayed for tution partners
    Given a user is using a 'phone'
    Given a user has arrived on the 'Tuition Partner' page for 'nudge-education'
    Then the logo is not shown

Scenario: No LA label shown if go to TP details via All TP pages
    Given a user has arrived on the 'Tuition Partner' page for 'bright-heart-education'
    Then the LA name is not shown

Scenario: LA label shown if go to TP details via search results page
    Given a user has arrived on the 'Tuition Partner' page for 'Bright Heart Education' after searching for 'Key stage 1 English' in postcode 'sk11eb'
    Then the LA name displayed is 'Stockport'