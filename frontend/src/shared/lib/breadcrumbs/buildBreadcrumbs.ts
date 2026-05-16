export function buildBreadcrumbs(path: string) {
  if (!path) return [];

  const parts = path.split(".");

  return parts.map((part, index) => {
    const href =
      "/departments/" +
      parts.slice(0, index + 1).join("/");

    const label = part
      .split("-")
      .map(
        (w) =>
          w.charAt(0).toUpperCase() +
          w.slice(1),
      )
      .join(" ");

    return {
      label,
      href,
      isLast: index === parts.length - 1,
    };
  });
}