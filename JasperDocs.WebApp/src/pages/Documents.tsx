import { Title, Text, Container, Paper, Stack, Button, Group, Modal, FileInput } from '@mantine/core';
import { IconPlus, IconUpload } from '@tabler/icons-react';
import { useState } from 'react';
import { usePostApiDocuments } from '../api/generated/documents/documents';

export function Documents() {
  const [opened, setOpened] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const createDocument = usePostApiDocuments();

  const handleSubmit = async () => {
    if (!file) return;

    try {
      await createDocument.mutateAsync({ data: { File: file } });
      setOpened(false);
      setFile(null);
    } catch (error) {
      console.error('Failed to create document:', error);
    }
  };

  return (
    <Container size="lg" py="xl">
      <Stack gap="lg">
        <Group justify="space-between">
          <Title order={1}>Documents</Title>
          <Button leftSection={<IconPlus size={16} />} onClick={() => setOpened(true)}>
            New Document
          </Button>
        </Group>
        <Paper shadow="sm" p="md">
          <Text c="dimmed">
            No documents yet. Create your first document to get started.
          </Text>
        </Paper>
      </Stack>

      <Modal
        opened={opened}
        onClose={() => {
          setOpened(false);
          setFile(null);
        }}
        title="Upload Document"
      >
        <Stack>
          <FileInput
            label="Select file"
            placeholder="Choose a file to upload"
            leftSection={<IconUpload size={16} />}
            value={file}
            onChange={setFile}
            required
          />
          <Group justify="flex-end">
            <Button variant="subtle" onClick={() => setOpened(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={!file}
              loading={createDocument.isPending}
            >
              Upload
            </Button>
          </Group>
        </Stack>
      </Modal>
    </Container>
  );
}
