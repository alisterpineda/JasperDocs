import { Title, Container, Paper, Stack, Tabs, Center, Loader, Text, Box } from '@mantine/core'
import { useParams } from '@tanstack/react-router'
import { useGetApiDocumentsId } from '../api/generated/documents/documents'
import { useEffect, useState } from 'react'
import { AXIOS_INSTANCE } from '../api/axios-instance'

export function DocumentDetail() {
  const { documentId } = useParams({ from: '/documents/$documentId' })
  const { data, isLoading, isError, error } = useGetApiDocumentsId(documentId)
  const [blobUrl, setBlobUrl] = useState<string | null>(null)
  const [fileLoading, setFileLoading] = useState(false)
  const [fileError, setFileError] = useState<string | null>(null)

  const selectedVersion = data?.selectedVersion
  const isPdf = selectedVersion?.mimeType === 'application/pdf'

  // Fetch the file with authentication when the selected version changes
  useEffect(() => {
    if (!selectedVersion || !isPdf) {
      return
    }

    let objectUrl: string | null = null

    const fetchFile = async () => {
      setFileLoading(true)
      setFileError(null)

      try {
        const response = await AXIOS_INSTANCE.get(
          `/api/documents/versions/${selectedVersion.id}/file`,
          {
            responseType: 'blob',
          }
        )

        // Create a blob URL from the response
        const blob = new Blob([response.data], { type: selectedVersion.mimeType })
        const url = URL.createObjectURL(blob)
        objectUrl = url
        setBlobUrl(url)
      } catch (err) {
        setFileError(err instanceof Error ? err.message : 'Failed to load file')
      } finally {
        setFileLoading(false)
      }
    }

    fetchFile()

    // Cleanup blob URL when component unmounts or version changes
    return () => {
      if (objectUrl) {
        URL.revokeObjectURL(objectUrl)
      }
    }
  }, [selectedVersion, isPdf])

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
              {isPdf ? (
                <Box style={{ width: '100%', height: '800px' }}>
                  {fileLoading ? (
                    <Center p="xl">
                      <Loader />
                    </Center>
                  ) : fileError ? (
                    <Center p="xl">
                      <Text c="red">{fileError}</Text>
                    </Center>
                  ) : blobUrl ? (
                    <iframe
                      src={blobUrl}
                      style={{
                        width: '100%',
                        height: '100%',
                        border: 'none',
                      }}
                      title={`Preview of ${data.title}`}
                    />
                  ) : null}
                </Box>
              ) : (
                <Center p="xl">
                  <Stack align="center" gap="md">
                    <Text c="dimmed" size="lg">
                      Preview not supported for this file type
                    </Text>
                    <Text c="dimmed" size="sm">
                      MIME Type: {selectedVersion?.mimeType}
                    </Text>
                  </Stack>
                </Center>
              )}
            </Tabs.Panel>
          </Tabs>
        </Paper>
      </Stack>
    </Container>
  )
}
