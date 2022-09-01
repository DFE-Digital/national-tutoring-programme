import { Given, When, Then, Step } from "@badeball/cypress-cucumber-preprocessor";

Given("a user accesses a service page after accepting cookies", () => {
    Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are accepted");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("a user accesses a service page after rejecting cookies", () => {
    Step(this, "a user has started the 'Find a tuition partner' journey");
    Step(this, "cookies are rejected");
    Step(this, "the 'Which subjects' page is displayed");
    Step(this, "the banner disappears");
  });

Given("the 'Which subjects' page is displayed", () => {
    cy.visit("search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&KeyStages=KeyStage1");
});

Given("nothing is selected", () => {
  cy.get('[data-testid="cookie-consent-accept"]').should('not.be.checked');
  cy.get('[data-testid="cookie-consent-deny"]').should('not.be.checked');
});

Given("the success banner has been displayed", () => {
  Step(this, "the 'Which subjects' page is displayed");
  Step(this, "the 'cookies' is selected from footer");
  Step(this, "a user opts-in");
  Step(this, "Saves Changes");
  Step(this, "a Success Banner is displayed");
});

Given("the 'view cookies' page is displayed", () => {
  Step(this, "the 'Which subjects' page is displayed");
  Step(this, "the 'cookies' is selected from footer");
});

Given("opt-in is selected", () => {
  cy.get('[data-testid="cookie-consent-accept"]').get('input[id="Consent"]').should('be.checked');
});

Given("a user accessed the search result page", () => {
  cy.get('[data-testid="cookie-consent-accept"]').get('input[id="Consent"]').should('be.checked');
});

Given("the search result page is displayed", () => {
  cy.visit("search-results?Postcode=SK1%201EB&Subjects=KeyStage1-English&KeyStages=KeyStage1")
});
  
When("cookies are accepted", () => {
    cy.get('[data-testid="accept-cookies"]').click();
  });

When("cookies are rejected", () => {
    cy.get('[data-testid="reject-cookies"]').click();
  });

When("the 'view cookies' is selected", () => {
    cy.get('[data-testid="view-cookies"]').click();
  });

When("the 'cookies' is selected from footer", () => {
    cy.get('[data-testid="view-footer-cookies"]').click();
  });

When("a user opts-in", () => {
  cy.get('[data-testid="cookie-consent-accept"]').click();
  });

When("a user opts-out", () => {
    cy.get('[data-testid="cookie-consent-deny"]').click();
    });

When("Saves Changes", () => {
    cy.get('[data-testid="call-to-action"]').click();
    });

When("the link to previous page is clicked", () => {
      cy.get('[data-testid="view-previous-page-link"]').click();
      });

Then("the cookies banner is displayed", () => {
    cy.get('[data-testid="cookie-banner"]').should("exist");
  });

Then("user session is tracked", () => {
    cy.get('head script').should('contain', 'gtag')
  });

Then("the banner disappears", () => {
    cy.contains('[data-testid="cookie-banner"]').should('not.exist');;
  });
  
Then("user session is not tracked", () => {
    cy.visit("search-results?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&KeyStages=KeyStage1");
    cy.contains('gtag').should('not.exist');;
  });

Then("the 'view cookies' page is loaded", () => {
    cy.location('pathname').should('eq', '/cookies');
  });

Then("the cookie banner is not displayed", () => {
    Step(this, "the banner disappears");
  });

Then("a Success Banner is displayed", () => {
    cy.get('[data-testid="success-banner"]').should('exist');;
  });

Then("the previous page is displayed correctly", () => {
  cy.location('search').should('eq', '?Postcode=sk11eb&Subjects=KeyStage1-English&Subjects=KeyStage1-Maths&Subjects=KeyStage1-Science&KeyStages=KeyStage1');
});
  
Then("the privacy policy is accessible in a new tab", () => {
  cy.get('[data-testid="privacy-policy-link"]').then(function ($a) {
    const href = $a.prop('href');
    cy.request(href).its('body').should('include', '</html>');
})});

Then("opt-out is selected", () => {
  cy.get('[data-testid="cookie-consent-deny"]').get('input[id="Consent-2"]').should('be.checked');
});

Then("the error banner is displayed", () => {
  cy.get('[id=error-summary-title]').should('exist');
  cy.get('.govuk-error-summary__list').first().should('contain.text', 'You must select an option');
});

Then("cookie {string} is added with value {string}", (cookie, value) => {
  if (value != 'null')
  {
    cy.getCookie(cookie)
    .should('have.property', 'value', value)
  }
  else
  {
    cy.getCookie(cookie).should('exist')
  }
  
});

Given("a user has arrived on the funding and reporting page", () => {
    cy.visit(`/funding-and-reporting`);
});

Then("the user redirected to funding page", () => {
    cy.location('pathname').should('eq', '/funding-and-reporting');
});

