export const kebabCase = (string) =>
  string
    .replace(
      /((?<=[a-z])[A-Z]|(?<=[^\-\W])[A-Z](?=[a-z])|(?<=[a-z])\d+|(?<=\d+)[a-z])/g,
      " $1"
    )
    .trim()
    .replace(/[\s_]+/g, "-")
    .replace(/[^a-zA-Z0-9_{}()\-~/]/g, "")
    .replace(/[-]{2,}/g, "-")
    .toLowerCase();

export const removeWhitespace = (string) => string.trim().replace(/\s/, "");

export const camelCaseKeyStage = (s) => {
  switch (s) {
    case "Key stage 1":
      return "KeyStage1";
    case "Key stage 2":
      return "KeyStage2";
    case "Key stage 3":
      return "KeyStage3";
    case "Key stage 4":
      return "KeyStage4";
    default:
      return "";
  }
};

export const KeyStageSubjects = (prefix, input) =>
  input
    .split(",")
    .map((s) => s.trim())
    .map((s) => {
      const endOfKs = s.lastIndexOf(" ");
      const ks = camelCaseKeyStage(s.slice(0, endOfKs));
      const subj = s.slice(endOfKs + 1, s.length);
      return `${prefix}=${ks}-${subj}`;
    })
    .join("&");

export const removeNewLine = (text) => text.trim().replace(/[\r\n]/gm, "");
export const removeExcessWhitespaces = (text) =>
  text.trim().replace(/  +/g, " ");
