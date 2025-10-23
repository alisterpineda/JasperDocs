import { Title, Text, Container, Paper, Stack, Button, Group } from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';

export function Documents() {
  return (
    <Container size="lg" py="xl">
      <Stack gap="lg">
        <Group justify="space-between">
          <Title order={1}>Documents</Title>
          <Button leftSection={<IconPlus size={16} />}>
            New Document
          </Button>
        </Group>
        <Paper shadow="sm" p="md">
          <Text c="dimmed">
            No documents yet. Create your first document to get started.
          </Text>
        </Paper>
      </Stack>
    </Container>
  );
}
