import { Title, Container, Paper, Stack, Tabs, Center, Loader, Text } from '@mantine/core'
import { useParams } from '@tanstack/react-router'
import { useGetApiDocumentsId } from '../api/generated/documents/documents'

export function DocumentDetail() {
  const { documentId } = useParams({ from: '/documents/$documentId' })
  const { data, isLoading, isError, error } = useGetApiDocumentsId(documentId)

  if (isLoading) {
    return (
      <Container size="lg" py="xl">
        <Center p="xl">
          <Loader />
        </Center>
      </Container>
    )
  }

  if (isError) {
    return (
      <Container size="lg" py="xl">
        <Text c="red">Failed to load document: {error?.message || 'Unknown error'}</Text>
      </Container>
    )
  }

  if (!data) {
    return (
      <Container size="lg" py="xl">
        <Text c="dimmed">Document not found</Text>
      </Container>
    )
  }

  return (
    <Container size="lg" py="xl">
      <Stack gap="lg">
        <Title order={1}>{data.title}</Title>

        <Paper shadow="sm" p="md">
          <Tabs defaultValue="preview">
            <Tabs.List>
              <Tabs.Tab value="preview">Preview</Tabs.Tab>
            </Tabs.List>

            <Tabs.Panel value="preview" pt="md">
              {/* Preview content will be implemented later */}
            </Tabs.Panel>
          </Tabs>
        </Paper>
      </Stack>
    </Container>
  )
}
