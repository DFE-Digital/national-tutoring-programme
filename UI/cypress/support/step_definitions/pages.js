import { Given, When, Then } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has started the 'Find a tuition partner' journey", () => {
  cy.visit("/");
});

Given("a user has arrived on the funding and reporting page", () => {
  cy.visit(`/funding-and-reporting`);
});

Given("a user has arrived on the academic mentors page", () => {
  cy.visit(`/academic-mentors`);
});

Given("a user has arrived on the school led tutoring page", () => {
  cy.visit(`/school-led-tutoring`);
});

Given("a user has arrived on the contact us page", () => {
  cy.visit(`/contact-us`);
});

Given("a user has arrived on the Report issues page", () => {
  cy.visit(`/report-issues`);
});

Given("a user has arrived on the accessibility page", () => {
  cy.visit(`/accessibility`);
});

Given("a user has arrived on the cookies page", () => {
  cy.visit(`/cookies`);
});

Given("a user has arrived on the privacy page", () => {
  cy.visit(`/privacy`);
});

When(
  "they set the {string} query string parameter value to {string}",
  (key, value) => {
    cy.location("pathname").then((pathName) => {
      const separator = pathName.includes("?") === true ? "&" : "?";
      const url = `${pathName}${separator}${key}=${value}`;
      cy.visit(url);
    });
  }
);

Then(
  "they will be taken to the 'Find a tuition partner' journey start page",
  () => {
    cy.location("pathname").should("eq", "/");
  }
);

Then("they will be taken to the 'Which key stages' page", () => {
  cy.location("pathname").should("eq", "/which-key-stages");
});

Then("they will be taken to the 'Which subjects' page", () => {
  cy.location("pathname").should("eq", "/which-subjects");
});

Then("they will be taken to the 'Search Results' page", () => {
  cy.location("pathname").should("eq", "/search-results");
});

Then("the page URL ends with {string}", (url) => {
  cy.location("pathname").should("match", new RegExp(`${url}$`));
});

Then("the heading should say {string}", (heading) => {
  cy.get("h1").should("have.text", heading);
});

Then("the page's title is {string}", (title) => {
  cy.title().should("eq", title);
});

Then("they will click the contact us link", () => {
  cy.get('[data-testid="contact-us-link"]').click();
});
