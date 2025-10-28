import { Badge } from '@mantine/core';

interface PartyBadgeProps {
  name: string;
}

export function PartyBadge({ name }: PartyBadgeProps) {
  return (
    <Badge variant="light" color="blue" size="sm">
      {name}
    </Badge>
  );
}
